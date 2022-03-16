
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
    public class DownloadDocumentWithUrlUseCaseTests
    {
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private DownloadDocumentWithUrlUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DownloadDocumentWithUrlUseCase(_s3Gateway.Object, _documentsGateway.Object);
        }

        [Test]
        public void ReturnsTheDownloadUrl()
        {
            var documentId = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument();
            document.Id = documentId;
            var downloadUrl = "www.fakeS3url.com";

            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
            _s3Gateway.Setup(x => x.GeneratePreSignedDownloadUrl(It.IsAny<Document>())).Returns(downloadUrl);

            var result = _classUnderTest.Execute(documentId.ToString());

            result.Should().BeEquivalentTo(downloadUrl);
            _documentsGateway.VerifyAll();
            _s3Gateway.VerifyAll();
        }

        [Test]
        public void ThrowsNotFoundIfDocumentDoesNotExist()
        {
            var documentId = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument();
            document.Id = documentId;

            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(null as Document);

            Func<string> testDelegate = () => _classUnderTest.Execute(documentId.ToString());

            testDelegate.Should().Throw<NotFoundException>().WithMessage($"Cannot find document with ID: {documentId}");
        }

    }
}
