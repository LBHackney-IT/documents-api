using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class CreateClaimAndUploadDocumentUseCaseTests
    {
        private readonly CreateClaimAndUploadDocumentUseCase _classUnderTest;
        private readonly Mock<ICreateClaimUseCase> _createClaimUseCase = new Mock<ICreateClaimUseCase>();
        private readonly Mock<IUploadDocumentUseCase> _uploadDocumentUseCase = new Mock<IUploadDocumentUseCase>();
        private readonly Mock<ILogger<CreateClaimAndUploadDocumentUseCase>> _logger = new Mock<ILogger<CreateClaimAndUploadDocumentUseCase>>();

        public CreateClaimAndUploadDocumentUseCaseTests()
        {
            _classUnderTest = new CreateClaimAndUploadDocumentUseCase(_createClaimUseCase.Object, _uploadDocumentUseCase.Object, _logger.Object);
        }

        [Test]
        public void ThrowsErrorWithInvalidRequest()
        {
            var request = new ClaimAndUploadDocumentRequest();

            Action execute = () => _classUnderTest.Execute(request);

            execute.Should().Throw<BadRequestException>();

        }
    }
}
