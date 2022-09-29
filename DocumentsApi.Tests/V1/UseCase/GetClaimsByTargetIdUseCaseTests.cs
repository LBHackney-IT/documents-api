using System;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Request;
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
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10
            };
            var existingClaim = TestDataHelper.CreateClaim();
            var gatewayResponse = new List<Claim>() { existingClaim };
            _documentsGateway.Setup(x => x.FindClaimsByTargetId(It.IsAny<Guid>(), It.IsAny<int>())).Returns(new List<Claim>(gatewayResponse));
            var expected = new Dictionary<string, List<ClaimResponse>>()
            {
                { "claims", new List<ClaimResponse>() { existingClaim.ToResponse() } }
            };

            var result = _classUnderTest.Execute(request);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ReturnsEmptyCollectionWhenNoClaimsWereFoundForTargetId()
        {
            _documentsGateway.Setup(x => x.FindClaimsByTargetId(It.IsAny<Guid>(), It.IsAny<int>())).Returns(new List<Claim>());

            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10
            };
            var result = _classUnderTest.Execute(request);

            result["claims"].Should().BeEmpty();
        }
    }
}
