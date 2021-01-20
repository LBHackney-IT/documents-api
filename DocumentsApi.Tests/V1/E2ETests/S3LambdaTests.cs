using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class S3LambdaTests : DatabaseTests
    {
        private S3EntryPoint _handler;
        private readonly Mock<ILambdaContext> _lambdaContext = new Mock<ILambdaContext>();
        private const string _contentType = "image/png";

        [SetUp]
        public void SetUp()
        {
            _lambdaContext.Setup(x => x.AwsRequestId).Returns("request ID");

            _handler = new S3EntryPoint(services =>
            {
                services.Replace(ServiceDescriptor.Singleton(x => DatabaseContext));
            });
        }

        [Test]
        public async Task UpdatesDocuments()
        {
            var document = TestDataHelper.CreateDocument().ToEntity();
            document.FileSize = 0;
            document.FileType = null;
            document.UploadedAt = null;

            DatabaseContext.Add(document);
            DatabaseContext.SaveChanges();

            await CreateTestS3Object(document.Id.ToString()).ConfigureAwait(true);
            var s3Event = TestDataHelper.CreateS3Event(new List<Guid> { document.Id, Guid.NewGuid() });

            await _handler.DocumentCreated(s3Event, _lambdaContext.Object).ConfigureAwait(true);

            DatabaseContext.Entry(document).State = EntityState.Detached;
            var found = DatabaseContext.Documents.First();

            var record = s3Event.Records.First();
            found.UploadedAt.Should().Be(record.EventTime);
            found.FileSize.Should().Be(record.S3.Object.Size);
            found.FileType.Should().Be(_contentType);

            DatabaseContext.Documents.Count().Should().Be(1);
        }

        private static async Task CreateTestS3Object(string id)
        {
            var options = AppOptions.FromEnv();
            var s3Config = new AmazonS3Config() { ServiceURL = options.AwsS3Endpoint };
            var client = new AmazonS3Client(s3Config);

            try
            {
                await client.PutBucketAsync(new PutBucketRequest { BucketName = options.DocumentsBucketName })
                    .ConfigureAwait(true);
            }
            catch (AmazonS3Exception)
            {
                Console.WriteLine("Test bucket already exists");
            }

            await client.PutObjectAsync(new PutObjectRequest
            {
                Key = id,
                BucketName = options.DocumentsBucketName,
                ContentBody = "test file",
                ContentType = _contentType
            }).ConfigureAwait(true);
        }
    }
}
