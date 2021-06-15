using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Serialization.Json;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace DocumentsApi
{
    public class S3EntryPoint
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ILogger<S3EntryPoint> _logger;

        public S3EntryPoint(ILogger<S3EntryPoint> logger)
        {
            var serviceCollection = new ServiceCollection();
            ServiceConfigurator.ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _logger = logger;
        }

        public S3EntryPoint(Action<ServiceCollection> configureTestServices)
        {
            var serviceCollection = new ServiceCollection();
            ServiceConfigurator.ConfigureServices(serviceCollection);
            configureTestServices?.Invoke(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public async Task DocumentCreated(S3Event s3Event, ILambdaContext context)
        {
            _logger.LogInformation("Processing event with Request ID {Id}", context.AwsRequestId);
            var useCase = _serviceProvider.GetService<IUpdateUploadedDocumentUseCase>();
            await useCase.ExecuteAsync(s3Event).ConfigureAwait(true);
            _logger.LogInformation("Processed event with Request ID {Id}", context.AwsRequestId);
        }
    }
}
