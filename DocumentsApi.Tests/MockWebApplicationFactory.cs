using System.Data.Common;
using Amazon.S3;
using DocumentsApi.V1.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentsApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly DbConnection _connection;
        private readonly IAmazonS3 _mockS3Client;

        public MockWebApplicationFactory(DbConnection connection, IAmazonS3 mockS3Client)
        {
            _connection = connection;
            _mockS3Client = mockS3Client;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                var dbBuilder = new DbContextOptionsBuilder();
                dbBuilder.UseNpgsql(_connection);
                var context = new DocumentsContext(dbBuilder.Options);
                services.AddSingleton(context);

                var serviceProvider = services.BuildServiceProvider();
                var dbContext = serviceProvider.GetRequiredService<DocumentsContext>();

                dbContext.Database.EnsureCreated();
            });

            builder.ConfigureTestServices(services =>
            {
                var options = new AppOptions();
                services.AddSingleton<AppOptions>(x => options);
                services.AddTransient(x => _mockS3Client);
            });
        }
    }
}
