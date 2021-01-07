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

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class CreateClaimUseCaseTests
    {
        private CreateClaimUseCase _classUnderTest;
        private Mock<IDocumentsGateway> _documentsGateway;
        private Fixture _fixture = new Fixture();
        private Document _document;
        private Claim _claim;

        public CreateClaimUseCaseTests()
        {
            _documentsGateway = new Mock<IDocumentsGateway>();
            _classUnderTest = new CreateClaimUseCase(_documentsGateway.Object);

            SetupMocks();
        }

        [Test]
        public void CreatesAClaim()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, DateTime.Now.AddDays(1))
                .Create();

            Func<ClaimResponse> execute = () => _classUnderTest.Execute(request);

            execute.Should().NotThrow();

            _documentsGateway.VerifyAll();
        }

        [Test]
        public void ReturnsAClaimResponse()
        {
            var request = _fixture.Build<ClaimRequest>()
                .With(x => x.RetentionExpiresAt, DateTime.Now.AddDays(1))
                .Create();

            var result = _classUnderTest.Execute(request);

            result.Should().BeEquivalentTo(_claim);
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
