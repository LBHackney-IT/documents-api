using System;
using System.Linq;
using AutoFixture;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class DocumentsContextTests : DatabaseTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = _fixture.Build<DocumentEntity>()
                .Without(x => x.Claims)
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();

            DatabaseContext.Documents.Add(databaseEntity);
            DatabaseContext.SaveChanges();

            var result = DatabaseContext.Documents.ToList().FirstOrDefault();

            result.Should().Be(databaseEntity);
            result?.Id.Should().NotBeEmpty();
            result?.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }

        [Test]
        public void CanSaveRelatedEntities()
        {
            var entity = _fixture.Build<ClaimEntity>()
                .With(x => x.Document, new DocumentEntity())
                .Create();

            DatabaseContext.Claims.Add(entity);
            DatabaseContext.SaveChanges();

            entity.Id.Should().NotBeEmpty();
            entity.CreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
            entity.Document.Id.Should().NotBeEmpty();

            DatabaseContext.Documents.ToList().First().Should().Be(entity.Document);
        }
    }
}
