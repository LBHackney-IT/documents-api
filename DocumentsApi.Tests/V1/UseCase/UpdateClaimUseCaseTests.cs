using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class UpdateClaimUseCaseTests
    {
        private UpdateClaimUseCase _classUnderTest;
        private Mock<IDocumentsGateway> _documentsGateway;
        private Mock<ILogger<UpdateClaimUseCase>> _logger;

        [SetUp]
        public void SetUp()
        {
            _documentsGateway = new Mock<IDocumentsGateway>();
            _logger = new Mock<ILogger<UpdateClaimUseCase>>();
            _classUnderTest = new UpdateClaimUseCase(_documentsGateway.Object, _logger.Object);
        }

        [Test]
        public void ReturnsTheUpdatedClaim()
        {
            // Arrange
            var id = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var claim = TestDataHelper.CreateClaim();
            claim.Id = id;
            claim.GroupId = groupId;
            _documentsGateway.Setup(x => x.FindClaim(id)).Returns(claim);
            var validUntil = DateTime.UtcNow.AddDays(5);
            var claimUpdateRequest = CreateClaimUpdateRequest(validUntil, groupId);

            // Act
            var result = _classUnderTest.Execute(id, claimUpdateRequest);

            // Assert
            result.Id.Should().Be(claim.Id);
            result.ValidUntil.Should().Be(validUntil);
            result.GroupId.Should().Be(groupId);
        }

        [Test]
        public void ThrowsAnErrorWhenRequestIsInvalid()
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

        [Test]
        public void ThrowsAnErrorWhenClaimDateIsInThePast()
        {
            // Arrange
            var id = Guid.NewGuid();
            var claimUpdateRequest = CreateClaimUpdateRequest(new DateTime(2000, 3, 20));
            _documentsGateway.Setup(x => x.FindClaim(id)).Returns(TestDataHelper.CreateClaim);

            // Act
            Action act = () => _classUnderTest.Execute(id, claimUpdateRequest);

            // Assert
            act.Should().Throw<BadRequestException>().WithMessage("The date cannot be in the past.");
        }

        private static ClaimUpdateRequest CreateClaimUpdateRequest(DateTime validUntil, Guid? groupId = null)
        {
            return new ClaimUpdateRequest()
            {
                ValidUntil = validUntil,
                GroupId = groupId
            };
        }
    }
}
