using System;
using Amazon.S3;
using Amazon.S3.Model;
using DocumentsApi.V1.Gateways;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class S3GatewayTests
    {
        private Mock<IAmazonS3> _s3;
        private S3Gateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _s3 = new Mock<IAmazonS3>();
            _classUnderTest = new S3Gateway(_s3.Object);
        }

        [Test]
        public void CanGeneratePresignedUploadUrl()
        {
            var document = TestDataHelper.CreateDocument();
            var expectedUrl = "https://presigned.url.com/foo";
            _s3
                .Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
                .Returns(expectedUrl);

            var result = _classUnderTest.GenerateUploadUrl(document);
            result.Should().Be(expectedUrl);

        }

        [Test]
        public void GeneratingUploadUrlCallsS3ClientCorrectly()
        {
            var document = TestDataHelper.CreateDocument();
            var expectedUrl = "https://presigned.url.com/foo";
            _s3
                .Setup(x => x.GetPreSignedURL(It.Is<GetPreSignedUrlRequest>(r =>
                    r.IsSetExpires() &&
                    r.BucketName == "testBucketName" &&
                    r.Key == document.Id.ToString() &&
                    r.Verb == HttpVerb.PUT
                )))
                .Returns(expectedUrl);

            _classUnderTest.GenerateUploadUrl(document);

            _s3.VerifyAll();
        }

        [Test]
        public void UploadUrlHasCorrectExpiry()
        {
            var document = TestDataHelper.CreateDocument();
            var expectedUrl = "https://presigned.url.com/foo";
            _s3
                .Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>()))
                .Returns(expectedUrl)
                .Callback<GetPreSignedUrlRequest>((request =>
                    request.Expires.Should().BeCloseTo(DateTime.Now.AddMinutes(S3Gateway.UrlExpiryMinutes))));

            _classUnderTest.GenerateUploadUrl(document);
        }
    }
}
