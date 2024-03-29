using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Jering.Javascript.NodeJS;
using AutoFixture;
using DocumentsApi.V1.Gateways;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using DocumentsApi.V1.Domain;
using Newtonsoft.Json;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class S3GatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IAmazonS3> _s3 = new Mock<IAmazonS3>();
        private readonly Mock<INodeJSService> _nodeJSService = new Mock<INodeJSService>();
        private S3Gateway _classUnderTest;
        private AppOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = _fixture.Create<AppOptions>();
            _classUnderTest = new S3Gateway(_s3.Object, _nodeJSService.Object, _options);
        }

        [Test]
        public async Task CanGeneratePresignedUploadUrl()
        {
            var document = _fixture.Create<Document>();

            var expectedPolicy = _fixture.Create<S3UploadPolicy>();
            var expectedPolicyString = JsonConvert.SerializeObject(expectedPolicy);

            _nodeJSService
                .Setup(x => x.InvokeFromFileAsync<string>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedPolicyString);

            var result = await _classUnderTest.GenerateUploadPolicy(document).ConfigureAwait(true);

            result.Should().BeEquivalentTo(expectedPolicy);

            _nodeJSService.VerifyAll();
        }

        [Test]
        public void CanGeneratePresignedDownloadUrl()
        {
            var document = _fixture.Create<Document>();
            document.UploadedAt = DateTime.UtcNow;
            var expectedDownloadUrl = "www.s3downloadurl.com";
            _s3.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(expectedDownloadUrl);

            var result = _classUnderTest.GeneratePreSignedDownloadUrl(document);
            result.Should().Be(expectedDownloadUrl);
        }

        [Test]
        public void ThrowsExceptionWhenCannotGenerateDownloadUrl()
        {
            var document = _fixture.Create<Document>();
            _s3.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Throws(new AmazonS3Exception("Error retrieving download url"));
            Func<string> testDelegate = () => _classUnderTest.GeneratePreSignedDownloadUrl(document);
            testDelegate.Should().Throw<AmazonS3Exception>();
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
    }
}
