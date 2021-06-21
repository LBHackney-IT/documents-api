using System;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class UploadDocumentUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Mock<ILogger<UploadDocumentUseCase>> _logger = new Mock<ILogger<UploadDocumentUseCase>>();
        private UploadDocumentUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new UploadDocumentUseCase(_s3Gateway.Object, _documentsGateway.Object, _logger.Object);
        }

        [Test]
        public void CanUploadDocument()
        {
            // Arrange
            DocumentUploadRequest request = new DocumentUploadRequest();
            var expectedS3Response = new PutObjectResponse();
            expectedS3Response.HttpStatusCode = HttpStatusCode.OK;
            var document = _fixture.Build<Document>()
                .Without(x => x.UploadedAt)
                .Create();
            _documentsGateway.Setup(x => x.FindDocument(request.Id)).Returns(document);
            _s3Gateway.Setup(x => x.UploadDocument(request)).Returns(expectedS3Response);

            // Act
            var result = _classUnderTest.Execute(request);

            // Assert
            result.Should().BeEquivalentTo(expectedS3Response.HttpStatusCode);
        }

        [Test]
        public void ThrowsAmazonS3ExceptionIfCannotUploadDocument()
        {
            // Arrange
            DocumentUploadRequest request = new DocumentUploadRequest();
            var document = _fixture.Build<Document>()
                .Without(x => x.UploadedAt)
                .Create();
            var expectedS3Response = new PutObjectResponse();
            expectedS3Response.HttpStatusCode = HttpStatusCode.OK;
            _documentsGateway.Setup(x => x.FindDocument(request.Id)).Returns(document);
            _s3Gateway.Setup(x => x.UploadDocument(request)).Throws(new AmazonS3Exception("Error retrieving the document"));

            // Act
            Func<HttpStatusCode> execute = () => _classUnderTest.Execute(request);

            // Assert
            execute.Should().Throw<AmazonS3Exception>();
        }

        [Test]
        public void ThrowsBadRequestErrorWhenDocumentAlreadyUploaded()
        {
            // Act
            var document = _fixture.Build<Document>()
                .With(x => x.FileSize, 1000)
                .With(x => x.FileType, "txt")
                .Create();
            DocumentUploadRequest request = new DocumentUploadRequest();
            request.Id = document.Id;

            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);

            // Act
            Func<HttpStatusCode> execute = () => _classUnderTest.Execute(request);

            // Assert
            execute.Should().Throw<BadRequestException>();
        }

        [Test]
        public void ThrowsNotFoundErrorWhenDocumentDoesNotExist()
        {
            // Arrange
            var request = new DocumentUploadRequest();
            request.Id = Guid.NewGuid();

            // Act
            Func<HttpStatusCode> execute = () => _classUnderTest.Execute(request);

            // Assert
            execute.Should().Throw<NotFoundException>();
        }
    }
}
