using System.Data;
using System.Data.Common;
using System.Net.Http;
using Amazon.S3;
using AutoFixture;
using AutoFixture.AutoMoq;
using DocumentsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Npgsql;
using NUnit.Framework;

namespace DocumentsApi.Tests
{
    public class IntegrationTests<TStartup> where TStartup : class
    {
        protected HttpClient Client { get; private set; }
        protected DocumentsContext DatabaseContext { get; private set; }
        protected Mock<IAmazonS3> MockS3Client { get; private set; }

        private MockWebApplicationFactory<TStartup> _factory;
        private NpgsqlConnection _connection;
        private DbTransaction _transaction;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
            _connection.Open();
            var npgsqlCommand = _connection.CreateCommand();
            npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
            npgsqlCommand.ExecuteNonQuery();
        }

        [SetUp]
        public void BaseSetup()
        {
            MockS3Client = CreateMockS3Client();
            _factory = new MockWebApplicationFactory<TStartup>(_connection, MockS3Client.Object);
            Client = _factory.CreateClient();
            DatabaseContext = _factory.Server.Host.Services.GetRequiredService<DocumentsContext>();

            _transaction = _connection.BeginTransaction(IsolationLevel.RepeatableRead);
            DatabaseContext.Database.UseTransaction(_transaction);
        }

        private static Mock<IAmazonS3> CreateMockS3Client()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var mockClient = fixture.Freeze<Mock<IAmazonS3>>();
            fixture.Create<IAmazonS3>();
            return mockClient;
        }

        [TearDown]
        public void BaseTearDown()
        {
            Client.Dispose();
            _factory.Dispose();
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
