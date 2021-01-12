using System;
using System.Threading.Tasks;
using AutoFixture;
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
    public class CreateUploadPolicyUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private CreateUploadPolicyUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new CreateUploadPolicyUseCase(_s3Gateway.Object, _documentsGateway.Object);
        }

        [Test]
        public void ThrowsNotFoundErrorWhenDocumentDoesNotExist()
        {
            Func<Task<S3UploadPolicy>> execute = async () => await _classUnderTest.Execute(Guid.NewGuid()).ConfigureAwait(true);
            execute.Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task ReturnsPresignedUrl()
        {
            var document = _fixture.Build<Document>()
                .Without(x => x.FileSize)
                .Without(x => x.FileType)
                .Create();

            var policy = _fixture.Create<S3UploadPolicy>();
            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);
            _s3Gateway.Setup(x => x.GenerateUploadPolicy(document)).ReturnsAsync(policy);

            var result = await _classUnderTest.Execute(document.Id).ConfigureAwait(true);

            result.Should().BeEquivalentTo(policy);

            _s3Gateway.VerifyAll();
        }

        [Test]
        public void ThrowsBadRequestErrorWhenDocumentAlreadyUploaded()
        {
            var document = _fixture.Build<Document>()
                .With(x => x.FileSize, 1000)
                .With(x => x.FileType, "txt")
                .Create();

            _documentsGateway.Setup(x => x.FindDocument(document.Id)).Returns(document);

            Func<Task<S3UploadPolicy>> execute = () => _classUnderTest.Execute(document.Id);
            execute.Should().ThrowAsync<BadRequestException>();
        }
    }
}
