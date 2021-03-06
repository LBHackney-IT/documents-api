using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using DocumentsApi.V1.Gateways;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class S3GatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IAmazonS3> _s3 = new Mock<IAmazonS3>();
        private S3Gateway _classUnderTest;
        private AppOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = _fixture.Create<AppOptions>();
            _classUnderTest = new S3Gateway(_s3.Object, _options);
        }

        [Test]
        public async Task CanGetObjectContentType()
        {
            var key = "i am an object key";
            var expectedContentType = "image/png";
            var response = new GetObjectMetadataResponse();
            response.Headers.ContentType = expectedContentType;

            _s3.Setup(x => x.GetObjectMetadataAsync(_options.DocumentsBucketName, key, default(CancellationToken))).ReturnsAsync(response);

            var result = await _classUnderTest.GetObjectContentType(key).ConfigureAwait(true);
            result.Should().Be(expectedContentType);
        }

        [Test]
        public void CanGetObject()
        {
            var documentId = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument();
            document.Id = documentId;
            document.UploadedAt = DateTime.UtcNow;
            var expected = new GetObjectResponse();
            _s3.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);
            var result = _classUnderTest.GetObject(document);
            result.Should().Be(expected);
        }

        [Test]
        public void ThrowsExceptionWhenCannotGetObject()
        {
            var document = TestDataHelper.CreateDocument();
            document.Id = Guid.NewGuid();
            _s3.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>())).Throws(new AmazonS3Exception("Error retrieving the document"));
            Func<GetObjectResponse> testDelegate = () => _classUnderTest.GetObject(document);
            testDelegate.Should().Throw<AmazonS3Exception>();
        }
    }
}
