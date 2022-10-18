using System;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetClaimsByTargetIdUseCaseTests
    {
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Mock<ILogger<GetClaimsByTargetIdUseCase>> _logger = new Mock<ILogger<GetClaimsByTargetIdUseCase>>();
        private GetClaimsByTargetIdUseCase _classUnderTest;

        public GetClaimsByTargetIdUseCaseTests()
        {
            _classUnderTest = new GetClaimsByTargetIdUseCase(_documentsGateway.Object, _logger.Object);
        }

        [Test]
        public void ReturnsClaimsByTargetIdWhenOnlyOnePage()
        {
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10
            };
            var existingClaim = TestDataHelper.CreateClaim();
            existingClaim.TargetId = request.TargetId;
            var gatewayResponse = new List<Claim>() { existingClaim };
            _documentsGateway.Setup(x => x.FindPaginatedClaimsByTargetId(request.TargetId, It.IsAny<int>(), null, null)).Returns(gatewayResponse);
            var expected = new PaginatedClaimResponse
            {
                Claims = new List<ClaimResponse>() { existingClaim.ToResponse() },
                Paging = new Paging
                {
                    Cursors = new Cursors
                    {
                        Before = "ewogICJpZCI6ICIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiCn0",
                        After = "ewogICJpZCI6ICIwMDAwMDAwMC0wMDAwLTAwMDAtMDAwMC0wMDAwMDAwMDAwMDAiCn0"
                    },
                    HasBefore = false,
                    HasAfter = false
                }
            };

            var result = _classUnderTest.Execute(request);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ThrowsErrorWhenPreviousAndNextPagesAreRequested()
        {
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10,
                Before = "asdswd",
                After = "zxcsxv"
            };

            Func<PaginatedClaimResponse> testDelegate = () => _classUnderTest.Execute(request);
            testDelegate.Should().Throw<BadRequestException>();
        }

        [Test]
        public void ThrowsErrorWhenUnableToDecodeAfterToken()
        {
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10,
                After = "sjkabkjsahkjscxzlcnxzlcnz"
            };

            Func<PaginatedClaimResponse> testDelegate = () => _classUnderTest.Execute(request);
            testDelegate.Should().Throw<BadRequestException>();
        }

        [Test]
        public void ThrowsErrorWhenUnableToDecodeBeforeToken()
        {
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10,
                Before = "sjkabkjsahkjscxzlcnxzlcnz"
            };

            Func<PaginatedClaimResponse> testDelegate = () => _classUnderTest.Execute(request);
            testDelegate.Should().Throw<BadRequestException>();
        }

        [Test]
        public void ReturnsEmptyCollectionWhenNoClaimsWereFoundForTargetId()
        {
            _documentsGateway.Setup(x => x.FindPaginatedClaimsByTargetId(It.IsAny<Guid>(), It.IsAny<int>(), null, null)).Returns(new List<Claim>());

            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 10
            };
            var result = _classUnderTest.Execute(request);

            result.Claims.Should().BeEmpty();
        }

        [Test]
        public void ReturnsClaimsByTargetIdWhenNextPageIsRequested()
        {
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 1,
                After = "eyJpZCI6IjcxYzE1MWY3LTE5MWEtNDY2YS1hOWMyLWE1NGYxNjJhNjRiZiJ9"
            };
            var cursor = new Guid("71c151f7-191a-466a-a9c2-a54f162a64bf"); // Decoded Guid from request
            var existingClaim1 = TestDataHelper.CreateClaim();
            existingClaim1.Id = new Guid("43166102-ff25-4bd0-ac7d-4f700a372413");
            existingClaim1.CreatedAt = new DateTime(2021, 10, 28);
            existingClaim1.TargetId = request.TargetId;
            var existingClaim2 = TestDataHelper.CreateClaim();
            existingClaim2.Id = new Guid("5aec02a5-15a4-4116-9fcf-b4351558cb70");
            existingClaim2.CreatedAt = new DateTime(2021, 9, 12);
            existingClaim2.TargetId = request.TargetId;
            var gatewayResponse = new List<Claim>() { existingClaim1, existingClaim2 };
            _documentsGateway.Setup(x => x.FindPaginatedClaimsByTargetId(request.TargetId, It.IsAny<int>(), cursor, true)).Returns(gatewayResponse);
            var expected = new PaginatedClaimResponse
            {
                Claims = new List<ClaimResponse>() { existingClaim1.ToResponse() },
                Paging = new Paging
                {
                    Cursors = new Cursors
                    {
                        Before = "ewogICJpZCI6ICI0MzE2NjEwMi1mZjI1LTRiZDAtYWM3ZC00ZjcwMGEzNzI0MTMiCn0",
                        After = "ewogICJpZCI6ICI0MzE2NjEwMi1mZjI1LTRiZDAtYWM3ZC00ZjcwMGEzNzI0MTMiCn0"
                    },
                    HasBefore = true,
                    HasAfter = true
                }
            };

            var result = _classUnderTest.Execute(request);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ReturnsClaimsByTargetIdWhenPreviousPageIsRequested()
        {
            var request = new PaginatedClaimRequest()
            {
                TargetId = Guid.NewGuid(),
                Limit = 1,
                Before = "eyJpZCI6IjcxYzE1MWY3LTE5MWEtNDY2YS1hOWMyLWE1NGYxNjJhNjRiZiJ9"
            };
            var cursor = new Guid("71c151f7-191a-466a-a9c2-a54f162a64bf"); // Decoded Guid from request
            var existingClaim1 = TestDataHelper.CreateClaim();
            existingClaim1.Id = new Guid("43166102-ff25-4bd0-ac7d-4f700a372413");
            existingClaim1.CreatedAt = new DateTime(2021, 10, 28);
            existingClaim1.TargetId = request.TargetId;
            var existingClaim2 = TestDataHelper.CreateClaim();
            existingClaim2.Id = new Guid("5aec02a5-15a4-4116-9fcf-b4351558cb70");
            existingClaim2.CreatedAt = new DateTime(2021, 9, 12);
            existingClaim2.TargetId = request.TargetId;
            var gatewayResponse = new List<Claim>() { existingClaim1, existingClaim2 };
            _documentsGateway.Setup(x => x.FindPaginatedClaimsByTargetId(request.TargetId, It.IsAny<int>(), cursor, false)).Returns(gatewayResponse);
            var expected = new PaginatedClaimResponse
            {
                Claims = new List<ClaimResponse>() { existingClaim2.ToResponse() },
                Paging = new Paging
                {
                    Cursors = new Cursors
                    {
                        Before = "ewogICJpZCI6ICI1YWVjMDJhNS0xNWE0LTQxMTYtOWZjZi1iNDM1MTU1OGNiNzAiCn0",
                        After = "ewogICJpZCI6ICI1YWVjMDJhNS0xNWE0LTQxMTYtOWZjZi1iNDM1MTU1OGNiNzAiCn0"
                    },
                    HasBefore = true,
                    HasAfter = true
                }
            };

            var result = _classUnderTest.Execute(request);

            result.Should().BeEquivalentTo(expected);
        }
    }
}
