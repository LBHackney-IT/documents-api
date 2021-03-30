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
            result?.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 1000);
        }

        [Test]
        public void CanSaveRelatedEntities()
        {
            var entity = _fixture.Build<ClaimEntity>()
                .With(x => x.Document, new DocumentEntity())
                .Create();

            DatabaseContext.Claims.Add(entity);
            DatabaseContext.SaveChanges();

            entity.Document.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 1000);
            entity.Document.Id.Should().NotBeEmpty();

            DatabaseContext.Documents.ToList().First().Should().Be(entity.Document);
        }

        [Test]
        public void DoesNotOverwriteExistingAttributes()
        {
            var id = Guid.NewGuid();
            var createdAt = DateTime.Today.AddDays(-3);
            var entity = _fixture.Build<DocumentEntity>()
                .Without(x => x.Claims)
                .With(x => x.Id, id)
                .With(x => x.CreatedAt, createdAt)
                .Create();

            DatabaseContext.Documents.Add(entity);
            DatabaseContext.SaveChanges();

            entity.Id.Should().Be(id);
            entity.CreatedAt.Should().Be(createdAt);
        }
    }
}
