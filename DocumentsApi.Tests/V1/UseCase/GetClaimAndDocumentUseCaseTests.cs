using System;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using DocumentsApi.V1.Domain;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using AutoFixture;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetClaimAndDocumentUseCaseTests
    {
        private readonly GetClaimAndPreSignedDownloadUrlUseCase _classUnderTest;
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private Fixture _fixture = new Fixture();

        public GetClaimAndDocumentUseCaseTests()
        {
            _classUnderTest = new GetClaimAndPreSignedDownloadUrlUseCase(_s3Gateway.Object, _documentsGateway.Object);
        }

        [Test]
        public void ReturnsCorrectResponse()
        {
            var existingClaim = _fixture.Create<Claim>();
            var s3DownloadUrl = new String("www.awsdownloadurl.com");
            _documentsGateway.Setup(x => x.FindClaim(It.IsAny<Guid>())).Returns(existingClaim);
            _s3Gateway.Setup(x => x.GeneratePreSignedDownloadUrl(existingClaim.Document)).Returns(s3DownloadUrl);

            var result = _classUnderTest.Execute(existingClaim.Id);

            var expectedResponse = _fixture.Build<ClaimAndPreSignedDownloadUrlResponse>()
                .With(x => x.ClaimId, existingClaim.Id)
                .With(x => x.PreSignedDownloadUrl, s3DownloadUrl)
                .Create();

            result.Should().BeOfType<ClaimAndPreSignedDownloadUrlResponse>();
            result.ClaimId.Should().Be(expectedResponse.ClaimId);
            result.PreSignedDownloadUrl.Should().Be(expectedResponse.PreSignedDownloadUrl);
        }

        [Test]
        public void ThrowsErrorWithInvalidRequest()
        {
            var nonExistingClaimId = Guid.NewGuid();
            _documentsGateway.Setup(x => x.FindClaim(nonExistingClaimId)).Returns(null as Claim);

            Func<ClaimAndPreSignedDownloadUrlResponse> testDelegate = () => _classUnderTest.Execute(nonExistingClaimId);

            testDelegate.Should().Throw<NotFoundException>();
        }
    }
}
