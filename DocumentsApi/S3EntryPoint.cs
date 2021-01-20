#nullable enable
using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Serialization.Json;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace DocumentsApi
{
    public class S3EntryPoint
    {
        private IUpdateUploadedDocumentUseCase _useCase;

        public S3EntryPoint(Action<ServiceCollection>? configureServices = null)
        {
            var serviceCollection = new ServiceCollection();
            ServiceConfigurator.ConfigureServices(serviceCollection);
            configureServices?.Invoke(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _useCase = serviceProvider.GetService<IUpdateUploadedDocumentUseCase>();
        }

        public async Task DocumentCreated(S3Event s3Event, ILambdaContext context)
        {
            Console.WriteLine($"Processing event with Request ID {context.AwsRequestId}");
            await _useCase.ExecuteAsync(s3Event).ConfigureAwait(true);
            Console.WriteLine($"Processed event with Request ID {context.AwsRequestId}");
        }
    }
}
