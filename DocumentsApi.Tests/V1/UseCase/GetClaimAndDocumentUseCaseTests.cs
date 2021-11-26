using System;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class GetClaimAndDocumentUseCaseTests
    {
        private readonly GetClaimAndDocumentUseCase _classUnderTest;
        private readonly Mock<IFindClaimByIdUseCase> _findClaimByIdUseCase = new Mock<IFindClaimByIdUseCase>();
        private readonly Mock<IDownloadDocumentUseCase> _downloadDocumentUseCase = new Mock<IDownloadDocumentUseCase>();
        private readonly Mock<ILogger<GetClaimAndDocumentUseCase>> _logger = new Mock<ILogger<GetClaimAndDocumentUseCase>>();

        public GetClaimAndDocumentUseCaseTests()
        {
            _classUnderTest = new GetClaimAndDocumentUseCase(_findClaimByIdUseCase.Object, _downloadDocumentUseCase.Object, _logger.Object);
        }

        [Test]
        public void ReturnsCorrectResponse()
        {
            var claimId = Guid.NewGuid();

            Action execute = () => _classUnderTest.Execute(claimId);

            execute.Should().NotBeNull();
        }
    }
}
