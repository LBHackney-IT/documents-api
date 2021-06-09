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

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class S3GatewayTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IAmazonS3> _s3 = new Mock<IAmazonS3>();
        private readonly Mock<INodeServices> _node = new Mock<INodeServices>();
        private S3Gateway _classUnderTest;
        private AppOptions _options;

        [SetUp]
        public void SetUp()
        {
            _options = _fixture.Create<AppOptions>();
            _classUnderTest = new S3Gateway(_s3.Object, _node.Object, _options);
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

        // [Test]
        // public void CanGeneratePreSignedDownloadUrl()
        // {
        //     var documentId = Guid.NewGuid();
        //     var document = TestDataHelper.CreateDocument();
        //     document.Id = documentId;
        //     document.UploadedAt = DateTime.UtcNow;
        //     var expected = "www.google.com";
        //     _s3.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(expected);
        //     var result = _classUnderTest.GeneratePreSignedDownloadUrl(document);
        //     result.Should().Be(expected);
        // }
        //
        // [Test]
        // public void ThrowsExceptionWhenCannotGenerateDownloadUrl()
        // {
        //     var document = TestDataHelper.CreateDocument();
        //     document.Id = Guid.NewGuid();
        //     _s3.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Throws(new AmazonS3Exception("Error retrieving download url"));
        //     Func<string> testDelegate = () => _classUnderTest.GeneratePreSignedDownloadUrl(document);
        //     testDelegate.Should().Throw<AmazonS3Exception>();
        // }
    }
}
