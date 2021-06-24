using System;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
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
        private readonly Mock<IDocumentFormatFactory> _documentFormatFactory = new Mock<IDocumentFormatFactory>();
        private readonly Mock<ILogger<UploadDocumentUseCase>> _logger = new Mock<ILogger<UploadDocumentUseCase>>();
        private UploadDocumentUseCase _classUnderTest;
        private Document _document;
        private Base64DecodedData _base64DecodedData;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new UploadDocumentUseCase(_s3Gateway.Object, _documentsGateway.Object, _documentFormatFactory.Object, _logger.Object);
            _document = _fixture.Build<Document>()
                .Without(x => x.UploadedAt)
                .Create();
            _base64DecodedData = _fixture.Build<Base64DecodedData>().Create();
            _documentFormatFactory.Setup(x => x.DecodeBase64DocumentString(It.IsAny<string>()))
                .Returns(_base64DecodedData);
        }

        [Test]
        public void CanUploadDocument()
        {
            // Arrange
            var request = new DocumentUploadRequest();
            var id = Guid.NewGuid();
            _documentsGateway.Setup(x => x.FindDocument(id)).Returns(_document);
            var expectedS3Response = new PutObjectResponse();
            expectedS3Response.HttpStatusCode = HttpStatusCode.OK;
            _s3Gateway.Setup(x => x.UploadDocument(id, _base64DecodedData)).Returns(expectedS3Response);

            // Act
            _classUnderTest.Execute(id, request);
        }

        [Test]
        public void ThrowsDocumentUploadExceptionIfAwsDoesNotReturn200()
        {
            // Arrange
            var request = new DocumentUploadRequest();
            var id = Guid.NewGuid();
            _documentsGateway.Setup(x => x.FindDocument(id)).Returns(_document);
            var expectedS3Response = new PutObjectResponse();
            expectedS3Response.HttpStatusCode = HttpStatusCode.OK;
            PutObjectResponse response = new PutObjectResponse();
            response.HttpStatusCode = HttpStatusCode.Forbidden;
            _s3Gateway.Setup(x => x.UploadDocument(id, _base64DecodedData)).Returns(response);

            // Act and assert
            Assert.Throws<DocumentUploadException>(() => _classUnderTest.Execute(id, request));
        }

        [Test]
        public void ThrowsAmazonS3ExceptionIfCannotUploadDocument()
        {
            // Arrange
            var request = new DocumentUploadRequest();
            var id = Guid.NewGuid();
            _documentsGateway.Setup(x => x.FindDocument(id)).Returns(_document);
            var expectedS3Response = new PutObjectResponse();
            expectedS3Response.HttpStatusCode = HttpStatusCode.OK;
            _s3Gateway.Setup(x => x.UploadDocument(id, _base64DecodedData)).Throws(new AmazonS3Exception("Error retrieving the document"));

            // Act and assert
            Assert.Throws<AmazonS3Exception>(() => _classUnderTest.Execute(id, request));
        }

        [Test]
        public void ThrowsBadRequestErrorWhenDocumentAlreadyUploaded()
        {
            // Act
            var already_uploaded_document = _fixture.Build<Document>()
                .With(x => x.FileSize, 1000)
                .With(x => x.FileType, "txt")
                .Create();
            DocumentUploadRequest request = new DocumentUploadRequest();
            _documentsGateway.Setup(x => x.FindDocument(already_uploaded_document.Id)).Returns(already_uploaded_document);

            // Act and assert
            Assert.Throws<BadRequestException>(() => _classUnderTest.Execute(already_uploaded_document.Id, request));
        }

        [Test]
        public void ThrowsNotFoundErrorWhenDocumentDoesNotExist()
        {
            // Arrange
            var request = new DocumentUploadRequest();
            var id = Guid.NewGuid();

            // Act and assert
            Assert.Throws<NotFoundException>(() => _classUnderTest.Execute(id, request));
        }
    }
}
