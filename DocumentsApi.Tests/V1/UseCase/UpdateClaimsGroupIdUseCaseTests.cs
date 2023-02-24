using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using DocumentsApi.V1.UseCase;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Boundary.Request;
using System.Collections.Generic;
using Moq;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class UpdateClaimsGroupIdUseCaseTests
    {
        private readonly UpdateClaimsGroupIdUseCase _classUnderTest;
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Fixture _fixture = new Fixture();

        public UpdateClaimsGroupIdUseCaseTests()
        {
            _classUnderTest = new UpdateClaimsGroupIdUseCase(_documentsGateway.Object);
        }

        [Test]
        public void CanUpdateClaimsGroupId()
        {
            SetupMocks();
            var oldGroupId = Guid.NewGuid();
            var newGroupId = Guid.NewGuid();
            var request = new ClaimsUpdateRequest() { OldGroupId = oldGroupId, NewGroupId = newGroupId };
            _classUnderTest.Execute(request);
            _documentsGateway.Verify(dg => dg.UpdateClaimsGroupId(oldGroupId, newGroupId));
        }

        private void SetupMocks()
        {
            var claim1 = TestDataHelper.CreateClaim();
            var claim2 = TestDataHelper.CreateClaim();
            var claim3 = TestDataHelper.CreateClaim();
            var claims = new List<Claim>() { claim1, claim2, claim3 };
            _documentsGateway
                .Setup(x => x.UpdateClaimsGroupId(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(claims);
        }
    }
}
