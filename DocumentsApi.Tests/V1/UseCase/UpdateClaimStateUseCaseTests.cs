using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class UpdateClaimStateUseCaseTests
    {
        private UpdateClaimStateUseCase _classUnderTest;
        private Mock<IDocumentsGateway> _documentsGateway;

        [SetUp]
        public void SetUp()
        {
            _documentsGateway = new Mock<IDocumentsGateway>();
            _classUnderTest = new UpdateClaimStateUseCase(_documentsGateway.Object);
        }

        [Test]
        public void ReturnsTheUpdatedClaim()
        {
            // Arrange
            var id = Guid.NewGuid();
            var claim = TestDataHelper.CreateClaim();
            claim.Id = id;
            _documentsGateway.Setup(x => x.FindClaim(id)).Returns(claim);
            var validUntil = DateTime.UtcNow.AddDays(5);
            var claimUpdateRequest = CreateClaimUpdateRequest(validUntil);

            // Act
            var result = _classUnderTest.Execute(id, claimUpdateRequest);

            // Assert
            result.Id.Should().Be(claim.Id);
            result.ValidUntil.Should().Be(validUntil);
        }

        [Test] public void ThrowsAnErrorWhenRequestIsInvalid()
        {
            // Arrange
            var id = Guid.NewGuid();
            _documentsGateway.Setup(x => x.FindClaim(id)).Returns(TestDataHelper.CreateClaim);

            // Act
            Action act = () => _classUnderTest.Execute(id, null);

            // Assert
            act.Should().Throw<BadRequestException>().WithMessage($"Cannot update Claim with ID: {id} because of invalid request.");
        }

        [Test]
        public void ThrowsAnErrorWhenClaimIsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var claimUpdateRequest = CreateClaimUpdateRequest(DateTime.UtcNow);

            // Act
            Action act = () => _classUnderTest.Execute(id, claimUpdateRequest);

            // Assert
            act.Should().Throw<NotFoundException>().WithMessage($"Could not find Claim with ID: {id}");
        }

        private static ClaimUpdateRequest CreateClaimUpdateRequest(DateTime validUntil)
        {
            return new ClaimUpdateRequest()
            {
                ValidUntil = validUntil
            };
        }
    }
}
