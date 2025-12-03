using System;
using System.Collections.Generic;
using Amazon;
using Amazon.S3;
using Jering.Javascript.NodeJS;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using DocumentsApi.V1.UseCase;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentsApi
{
    public static class ServiceConfigurator
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            var options = AppOptions.FromEnv();
            services.AddSingleton<AppOptions>(x => options);

            services.AddNodeJS();
            services.Configure<NodeJSProcessOptions>(o =>
            {
                o.ExecutablePath = "/opt/bin/node";
            });
            // var serviceProvider = services.BuildServiceProvider();
            // var nodeJSService = serviceProvider.GetRequiredService<INodeJSService>();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            // Database Context
            services.AddDbContext<DocumentsContext>(
                opt => opt.UseLazyLoadingProxies().UseNpgsql(options.DatabaseConnectionString));

            // Transient Services
            AmazonS3Config s3Config;
            if (options.AwsS3Endpoint != null) s3Config = new AmazonS3Config() { ServiceURL = options.AwsS3Endpoint };
            else s3Config = new AmazonS3Config() { RegionEndpoint = RegionEndpoint.EUWest2 };

            services.AddTransient<IAmazonS3>(sp => new AmazonS3Client(s3Config));

            // Gateways
            services.AddScoped<IDocumentsGateway, DocumentsGateway>();
            services.AddScoped<IS3Gateway, S3Gateway>();

            // Use Cases
            services.AddScoped<ICreateClaimUseCase, CreateClaimUseCase>();
            services.AddScoped<IUpdateUploadedDocumentUseCase, UpdateUploadedDocumentUseCase>();
            services.AddScoped<IFindClaimByIdUseCase, FindClaimByIdUseCase>();
            services.AddScoped<IUpdateClaimUseCase, UpdateClaimUseCase>();
            services.AddScoped<ICreateClaimAndS3UploadPolicyUseCase, CreateClaimAndS3UploadPolicyUseCase>();
            services.AddScoped<IGetClaimAndPreSignedDownloadUrlUseCase, GetClaimAndPreSignedDownloadUrlUseCase>();
            services.AddScoped<ICreateUploadPolicyUseCase, CreateUploadPolicyUseCase>();
            services.AddScoped<IGeneratePreSignedDownloadUrlUseCase, GeneratePreSignedDownloadUrlUseCase>();
            services.AddScoped<IGetClaimsByGroupIdUseCase, GetClaimsByGroupIdUseCase>();
            services.AddScoped<IUpdateClaimsGroupIdUseCase, UpdateClaimsGroupIdUseCase>();
        }
    }
}
