using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.NodeServices;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class S3GatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IAmazonS3> _s3 = new Mock<IAmazonS3>();
        private readonly Mock<INodeServices> _node = new Mock<INodeServices>();
        private readonly Mock<ILogger<S3Gateway>> _logger = new Mock<ILogger<S3Gateway>>();
        private S3Gateway _classUnderTest;
        private AppOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = _fixture.Create<AppOptions>();
            _classUnderTest = new S3Gateway(_s3.Object, _node.Object, _options, _logger.Object);
        }

        [Test]
        public async Task CanGeneratePresignedUploadUrl()
        {
            var document = TestDataHelper.CreateDocument();
            var expectedPolicy = _fixture.Create<S3UploadPolicy>();
            var expectedPolicyString = JsonConvert.SerializeObject(expectedPolicy);
            _node
                .Setup(x => x.InvokeAsync<string>("V1/Node/index.js", _options.DocumentsBucketName, "pre-scan/" + document.Id.ToString(), 3600))
                .ReturnsAsync(expectedPolicyString);

            var result = await _classUnderTest.GenerateUploadPolicy(document).ConfigureAwait(true);
            result.Should().BeEquivalentTo(expectedPolicy);

            _node.VerifyAll();
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
