using System;
using AutoFixture;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class CreateUploadUrlUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private CreateUploadUrlUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new CreateUploadUrlUseCase(_s3Gateway.Object, _documentsGateway.Object);
        }

        [Test]
        public void ThrowsNotFoundErrorWhenDocumentDoesNotExist()
        {
            Func<UrlResponse> execute = () => _classUnderTest.Execute(Guid.NewGuid());
            execute.Should().Throw<NotFoundException>();
        }

        [Test]
        public void CreatesPresignedUrl()
        {
            var document = _fixture.Build<Document>()
                .Without(x => x.FileSize)
                .Without(x => x.FileType)
                .Create();

            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
            _s3Gateway.Setup(x => x.GenerateUploadUrl(document));

            _classUnderTest.Execute(document.Id);

            _s3Gateway.VerifyAll();
        }

        [Test]
        public void ReturnsPresignedUrl()
        {
            var document = _fixture.Build<Document>()
                .Without(x => x.FileSize)
                .Without(x => x.FileType)
                .Create();

            var url = new Uri("https://upload.s3.com");
            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
            _s3Gateway.Setup(x => x.GenerateUploadUrl(document)).Returns(url);

            var result = _classUnderTest.Execute(document.Id);

            result.Url.Should().Be(url);
        }

        [Test]
        public void ThrowsBadRequestErrorWhenDocumentAlreadyUploaded()
        {
            var document = _fixture.Build<Document>()
                .With(x => x.FileSize, 1000)
                .With(x => x.FileType, "txt")
                .Create();

            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);

            Func<UrlResponse> execute = () => _classUnderTest.Execute(document.Id);
            execute.Should().Throw<BadRequestException>();
        }
    }
}
