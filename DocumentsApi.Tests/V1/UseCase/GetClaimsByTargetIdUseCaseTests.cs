using System;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetClaimsByTargetIdUseCaseTests
    {
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private GetClaimsByTargetIdUseCase _classUnderTest;

        public GetClaimsByTargetIdUseCaseTests()
        {
            _classUnderTest = new GetClaimsByTargetIdUseCase(_documentsGateway.Object);
        }

        [Test]
        public void ReturnsClaimsByTargetId()
        {
            var existingClaim = TestDataHelper.CreateClaim();
            var gatewayResponse = new List<Claim>() { existingClaim };
            _documentsGateway.Setup(x => x.FindClaimsByTargetId(It.IsAny<Guid>())).Returns(new List<Claim>(gatewayResponse));
            var expected = new Dictionary<string, List<ClaimResponse>>()
            {
                { "claims", new List<ClaimResponse>() { existingClaim.ToResponse() } }
            };

            var result = _classUnderTest.Execute(Guid.NewGuid());

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ReturnsEmptyWhenNoClaimsWereFoundForTargetId()
        {
            _documentsGateway.Setup(x => x.FindClaimsByTargetId(It.IsAny<Guid>())).Returns(null as List<Claim>);

            var result = _classUnderTest.Execute(Guid.NewGuid());

            result["claims"].Should().BeEmpty();
        }
    }
}
