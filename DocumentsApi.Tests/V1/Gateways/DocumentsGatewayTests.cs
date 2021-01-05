using System;
using System.Linq;
using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class DocumentsGatewayTests : DatabaseTests
    {
        private Fixture _fixture = new Fixture();
        private DocumentsGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DocumentsGateway(DatabaseContext);
        }

        [Test]
        public void CreatingADocumentShouldInsertIntoTheDatabase()
        {
            var request = _fixture.Build<Document>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();
            var query = DatabaseContext.Documents;

            _classUnderTest.CreateDocument(request);

            query.Count()
                .Should()
                .Be(1);

            var foundRecord = query.First();
            foundRecord.Id.Should().NotBeEmpty();
            foundRecord.FileSize.Should().Be(request.FileSize);
            foundRecord.FileType.Should().Be(request.FileType);
        }

        [Test]
        public void CreatingADocumentShouldReturnCreatedDocument()
        {
            var request = _fixture.Build<Document>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();

            var created = _classUnderTest.CreateDocument(request);

            created.FileSize.Should().Be(request.FileSize);
            created.FileType.Should().Be(request.FileType);
            created.Id.Should().NotBeEmpty();
            created.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Test]
        public void CreatingAClaimShouldInsertIntoTheDatabase()
        {
            var request = _fixture.Build<Claim>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();
            var query = DatabaseContext.Documents;

            _classUnderTest.CreateClaim(request);

            query.Count()
                .Should()
                .Be(1);

            var foundRecord = query.First();
            foundRecord.Id.Should().NotBeEmpty();
        }

        [Test]
        public void CreatingAClaimShouldReturnCreatedDocument()
        {
            var request = _fixture.Build<Claim>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();

            var created = _classUnderTest.CreateClaim(request);

            created.Id.Should().NotBeEmpty();
            created.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }
    }
}
