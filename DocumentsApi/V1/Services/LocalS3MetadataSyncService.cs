using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using DocumentsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DocumentsApi.V1.Services
{
    public class LocalS3MetadataSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AppOptions _options;

        public LocalS3MetadataSyncService(IServiceScopeFactory scopeFactory, AppOptions options)
        {
            _scopeFactory = scopeFactory;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SynchroniseDocuments(stoppingToken).ConfigureAwait(true);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Failed to synchronise local document metadata: {exception.Message}");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken).ConfigureAwait(true);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task SynchroniseDocuments(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DocumentsContext>();
                var s3 = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
                var documents = await context.Documents
                    .Where(document => document.UploadedAt == null)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(true);

                foreach (var document in documents)
                {
                    try
                    {
                        var metadata = await s3.GetObjectMetadataAsync(
                            _options.DocumentsBucketName,
                            "clean/" + document.Id,
                            cancellationToken).ConfigureAwait(true);

                        document.UploadedAt = metadata.LastModified.ToUniversalTime();
                        document.FileSize = metadata.ContentLength;
                        document.FileType = metadata.Headers.ContentType;
                    }
                    catch (AmazonS3Exception exception) when (exception.StatusCode == HttpStatusCode.NotFound)
                    {
                        // The browser has not finished uploading this document.
                    }
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(true);
            }
        }
    }
}
