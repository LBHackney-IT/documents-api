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
            _classUnderTest.Execute(request);
        }

        [Test]
        public void ThrowsDocumentUploadExceptionIfAwsDoesNotReturn200()
        {
            // Arrange
            DocumentUploadRequest request = new DocumentUploadRequest();
            var document = _fixture.Build<Document>()
                .Without(x => x.UploadedAt)
                .Create();
            var expectedS3Response = new PutObjectResponse();
            expectedS3Response.HttpStatusCode = HttpStatusCode.OK;
            _documentsGateway.Setup(x => x.FindDocument(request.Id)).Returns(document);
            PutObjectResponse response = new PutObjectResponse();
            response.HttpStatusCode = HttpStatusCode.Forbidden;
            _s3Gateway.Setup(x => x.UploadDocument(request)).Returns(response);

            // Act and assert
            Assert.Throws<DocumentUploadException>(() => _classUnderTest.Execute(request));
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

            // Act and assert
            Assert.Throws<AmazonS3Exception>(() => _classUnderTest.Execute(request));
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

            // Act and assert
            Assert.Throws<BadRequestException>(() => _classUnderTest.Execute(request));
        }

        [Test]
        public void ThrowsNotFoundErrorWhenDocumentDoesNotExist()
        {
            // Arrange
            var request = new DocumentUploadRequest();
            request.Id = Guid.NewGuid();

            // Act and assert
            Assert.Throws<NotFoundException>(() => _classUnderTest.Execute(request));
        }
    }
}
