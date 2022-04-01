using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class CreateClaimAndS3UploadPolicyUseCaseTests
    {
        private readonly CreateClaimAndS3UploadPolicyUseCase _classUnderTest;
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<ILogger<CreateClaimAndS3UploadPolicyUseCase>> _logger = new Mock<ILogger<CreateClaimAndS3UploadPolicyUseCase>>();

        public CreateClaimAndS3UploadPolicyUseCaseTests()
        {
            _classUnderTest = new CreateClaimAndS3UploadPolicyUseCase(_documentsGateway.Object, _s3Gateway.Object, _logger.Object);
        }

        [Test]
        public void ThrowsErrorWithInvalidRequest()
        {
            var request = new ClaimRequest();

            Func<Task<ClaimAndS3UploadPolicyResponse>> testDelegate = async () => await _classUnderTest.ExecuteAsync(request).ConfigureAwait(true);

            testDelegate.Should().Throw<BadRequestException>();

        }
    }
}
