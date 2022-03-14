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
    }
}