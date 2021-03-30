using System;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class FindClaimByIdUseCaseTests
    {
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private FindClaimByIdUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new FindClaimByIdUseCase(_documentsGateway.Object);
        }

        [Test]
        public void ReturnsClaimResponseWhenFound()
        {
            var found = TestDataHelper.CreateClaim();
            found.Id = Guid.NewGuid();
            _documentsGateway.Setup(x => x.FindClaim(found.Id)).Returns(found);

            var result = _classUnderTest.Execute(found.Id);

            result.Should().BeEquivalentTo(found, opts => opts.Excluding(c => c.Document.Uploaded).Excluding(c => c.Expired));
            result.Should().BeOfType<ClaimResponse>();

            _documentsGateway.VerifyAll();
        }

        [Test]
        public void ThrowsNotFoundIfClaimDoesNotExist()
        {
            Func<ClaimResponse> execute = () => _classUnderTest.Execute(Guid.NewGuid());

            execute.Should().Throw<NotFoundException>();
        }

        [Test]
        public void ThrowsNotFoundIfClaimHasExpired()
        {
            var claim = TestDataHelper.CreateClaim();
            claim.Id = Guid.NewGuid();
            claim.RetentionExpiresAt = DateTime.UtcNow.AddDays(-2);
            _documentsGateway.Setup(x => x.FindClaim(claim.Id)).Returns(claim);

            Func<ClaimResponse> execute = () => _classUnderTest.Execute(claim.Id);

            execute.Should().Throw<NotFoundException>();
        }
    }
}
