using System;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using DocumentsApi.V1.Domain;
using Amazon.S3;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class DownloadDocumentUseCaseTests
    {
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private DownloadDocumentUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DownloadDocumentUseCase(_s3Gateway.Object, _documentsGateway.Object);
        }

        // [Test]
        // public void ReturnsTheDownloadUrl()
        // {
        //     var documentId = Guid.NewGuid();
        //     var document = TestDataHelper.CreateDocument();
        //     document.Id = documentId;
        //     var downloadUrl = "www.google.com";
        //
        //     _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
        //     _s3Gateway.Setup(x => x.GeneratePreSignedDownloadUrl(It.IsAny<Document>())).Returns(downloadUrl);
        //
        //     var result = _classUnderTest.Execute(documentId.ToString());
        //
        //     result.Should().BeEquivalentTo(downloadUrl);
        //     _documentsGateway.VerifyAll();
        // }
        //
        // [Test]
        // public void ThrowsNotFoundIfDocumentDoesNotExist()
        // {
        //     var documentId = Guid.NewGuid();
        //     Func<string> execute = () => _classUnderTest.Execute(documentId.ToString());
        //
        //     execute.Should().Throw<NotFoundException>();
        // }
        //
        // [Test]
        // public void ThrowsAmazonS3ExceptionIfCannotRetrieveDownloadLink()
        // {
        //     var documentId = Guid.NewGuid();
        //     var document = TestDataHelper.CreateDocument();
        //     document.Id = documentId;
        //     _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
        //     _s3Gateway.Setup(x => x.GeneratePreSignedDownloadUrl(It.IsAny<Document>())).Throws(new AmazonS3Exception("Error retrieving the download link"));
        //     Func<string> execute = () => _classUnderTest.Execute(documentId.ToString());
        //
        //     execute.Should().Throw<AmazonS3Exception>();
        // }
    }
}
