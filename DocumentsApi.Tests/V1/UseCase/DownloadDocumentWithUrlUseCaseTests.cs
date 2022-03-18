
using System;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using DocumentsApi.V1.Domain;
using Amazon.S3;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class DownloadDocumentWithUrlUseCaseTests
    {
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private DownloadDocumentWithUrlUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DownloadDocumentWithUrlUseCase(_s3Gateway.Object, _documentsGateway.Object);
        }

        [Test]
        public void ReturnsTheDownloadUrl()
        {
            var claimId = Guid.NewGuid();
            var claim = TestDataHelper.CreateClaim();
            claim.Id = claimId;

            var downloadUrl = "www.fakeS3url.com";

            _documentsGateway.Setup(x => x.FindClaim(claim.Id)).Returns(claim);
            _s3Gateway.Setup(x => x.GeneratePreSignedDownloadUrl(claim.Document)).Returns(downloadUrl);

            var result = _classUnderTest.Execute(claimId);

            result.Should().BeEquivalentTo(downloadUrl);
            _documentsGateway.VerifyAll();
            _s3Gateway.VerifyAll();
        }

        [Test]
        public void ThrowsNotFoundIfClaimDoesNotExistForDownloadUrl()
        {
            var nonExistentClaimId = Guid.NewGuid();

            _documentsGateway.Setup(x => x.FindClaim(It.IsAny<Guid>())).Returns(null as Claim);

            Func<string> testDelegate = () => _classUnderTest.Execute(nonExistentClaimId);

            testDelegate.Should().Throw<NotFoundException>().WithMessage($"Cannot find a claim with ID: {nonExistentClaimId}");
        }

        [Test]
        public void ThrowsAmazonS3ExceptionIfCannotRetreiveDownloadLink()
        {
            var claimId = Guid.NewGuid();
            var claim = TestDataHelper.CreateClaim();
            claim.Id = claimId;

            _documentsGateway.Setup(x => x.FindClaim(claim.Id)).Returns(claim);
            _s3Gateway.Setup(x => x.GeneratePreSignedDownloadUrl(It.IsAny<Document>())).Throws(new AmazonS3Exception("Error when retreiving the download url"));
            Func<string> testDelegate = () => _classUnderTest.Execute(claimId);

            testDelegate.Should().Throw<AmazonS3Exception>();

        }

    }
}
