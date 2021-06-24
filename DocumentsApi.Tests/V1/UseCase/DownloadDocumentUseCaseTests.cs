using System;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Amazon.S3;
using Amazon.S3.Model;
using DocumentsApi.V1.Factories;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class DownloadDocumentUseCaseTests
    {
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Mock<IDocumentFormatFactory> _documentFormatFactory = new Mock<IDocumentFormatFactory>();
        private readonly Mock<ILogger<DownloadDocumentUseCase>> _logger = new Mock<ILogger<DownloadDocumentUseCase>>();
        private DownloadDocumentUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DownloadDocumentUseCase(_s3Gateway.Object, _documentsGateway.Object, _documentFormatFactory.Object, _logger.Object);
        }

        [Test]
        public void ReturnsTheDocumentInBase64()
        {
            var documentId = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument();
            document.Id = documentId;
            var expectedS3Response = new GetObjectResponse();
            var expectedResponse = "expectedResponse";
            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
            _s3Gateway.Setup(x => x.GetObject(document)).Returns(expectedS3Response);
            _documentFormatFactory.Setup(x => x.EncodeStreamToBase64(expectedS3Response)).Returns(expectedResponse);

            var result = _classUnderTest.Execute(documentId);

            result.Should().BeEquivalentTo(expectedResponse);
            _documentsGateway.VerifyAll();
        }

        [Test]
        public void ThrowsNotFoundIfDocumentDoesNotExist()
        {
            var documentId = Guid.NewGuid();
            Func<string> execute = () => _classUnderTest.Execute(documentId);

            execute.Should().Throw<NotFoundException>();
        }

        [Test]
        public void ThrowsAmazonS3ExceptionIfCannotRetrieveDocument()
        {
            var documentId = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument();
            document.Id = documentId;
            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
            _s3Gateway.Setup(x => x.GetObject(document)).Throws(new AmazonS3Exception("Error retrieving the document"));
            Func<string> execute = () => _classUnderTest.Execute(documentId);

            execute.Should().Throw<AmazonS3Exception>();
        }
    }
}
