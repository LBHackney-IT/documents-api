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
        private readonly ServiceProvider _serviceProvider;

        public S3EntryPoint()
        {
            var serviceCollection = new ServiceCollection();
            ServiceConfigurator.ConfigureServices(serviceCollection);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            _serviceProvider = serviceCollection.BuildServiceProvider();
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
            Console.WriteLine($"Processing event with Request ID {context.AwsRequestId}");
            var useCase = _serviceProvider.GetService<IUpdateUploadedDocumentUseCase>();
            await useCase.ExecuteAsync(s3Event).ConfigureAwait(true);
            Console.WriteLine($"Processed event with Request ID {context.AwsRequestId}");
        }
    }
}
