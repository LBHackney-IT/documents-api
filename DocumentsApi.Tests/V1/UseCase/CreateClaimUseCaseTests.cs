using System;
using AutoFixture;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class CreateClaimUseCaseTests
    {
        private readonly CreateClaimUseCase _classUnderTest;
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Mock<ILogger<CreateClaimUseCase>> _logger = new Mock<ILogger<CreateClaimUseCase>>();
        private readonly Fixture _fixture = new Fixture();
        private Document _document;
        private Claim _claim;

        public CreateClaimUseCaseTests()
        {
            _classUnderTest = new CreateClaimUseCase(_documentsGateway.Object, _logger.Object);

            SetupMocks();
        }

        [Test]
        public void CreatesAClaim()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, DateTime.UtcNow.AddDays(1))
                .With(x => x.TargetType, "Person")
                .Create();

            Func<ClaimResponse> execute = () => _classUnderTest.Execute(request);

            execute.Should().NotThrow();

            _documentsGateway.VerifyAll();
        }

        [Test]
        public void ReturnsAClaimResponse()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.TargetType, "Person")
                .With(x => x.RetentionExpiresAt, DateTime.UtcNow.AddDays(1))
                .Create();

            var result = _classUnderTest.Execute(request);

            result.Should().BeEquivalentTo(_claim, opt => opt.Excluding(x => x.Document.Uploaded).Excluding(x => x.Expired).Excluding(x => x.Document.Description));
            result.Document.Description.Should().Be(_claim.Document.Description);
        }

        [Test]
        public void ThrowsErrorWithInvalidRequest()
        {
            var request = new ClaimRequest();

            Func<ClaimResponse> execute = () => _classUnderTest.Execute(request);

            execute.Should().Throw<BadRequestException>();

        }

        private void SetupMocks()
        {
            _document = _fixture.Build<Document>()
                .Without(x => x.FileSize)
                .Without(x => x.FileType)
                .Create();

            _claim = _fixture.Build<Claim>()
                .With(x => x.Document, _document)
                .Create();

            _documentsGateway
                .Setup(x => x.CreateClaim(It.IsAny<Claim>()))
                .Returns(_claim);
        }
    }
}
