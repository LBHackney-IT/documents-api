using System;
using System.Linq;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Gateways
{
    [TestFixture]
    public class DocumentsGatewayTests : DatabaseTests
    {
        private DocumentsGateway _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new DocumentsGateway(DatabaseContext);
        }

        [Test]
        public void CreatingADocumentShouldInsertIntoTheDatabase()
        {
            var request = TestDataHelper.CreateDocument();
            var query = DatabaseContext.Documents;

            _classUnderTest.SaveDocument(request);

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
            var request = TestDataHelper.CreateDocument();

            var created = _classUnderTest.SaveDocument(request);

            created.FileSize.Should().Be(request.FileSize);
            created.FileType.Should().Be(request.FileType);
            created.Id.Should().NotBeEmpty();
            created.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Test]
        public void CreatingAClaimShouldInsertIntoTheDatabase()
        {
            var request = TestDataHelper.CreateClaim();
            var query = DatabaseContext.Claims;

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
            var request = TestDataHelper.CreateClaim();

            var created = _classUnderTest.CreateClaim(request);

            created.Id.Should().NotBeEmpty();
            created.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Test]
        public void CanFindADocument()
        {
            var document = TestDataHelper.CreateDocument().ToEntity();
            DatabaseContext.Add(document);
            DatabaseContext.SaveChanges();

            var found = _classUnderTest.FindDocument(document.Id);

            found.Should().BeEquivalentTo(document, opt => opt.ExcludingMissingMembers());
        }

        [Test]
        public void FindingADocumentReturnsNullWhenNotFound()
        {
            var found = _classUnderTest.FindDocument(Guid.NewGuid());

            found.Should().BeNull();
        }
    }
}
