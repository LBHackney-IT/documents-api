using AutoFixture;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Factories
{
    [TestFixture]
    public class DomainFactoryTests : DatabaseTests
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void CanMapDocumentEntityToDomainModel()
        {
            var domain = _fixture.Build<DocumentEntity>()
                .Without(x => x.Claims)
                .Create();
            var entity = domain.ToDomain();

            domain.Id.Should().Be(entity.Id);
            domain.CreatedAt.Should().Be(entity.CreatedAt);
            domain.FileSize.Should().Be(entity.FileSize);
            domain.FileType.Should().Be(entity.FileType);
            domain.Name.Should().Be(entity.Name);
        }

        [Test]
        public void CanMapClaimEntityToDomainModel()
        {
            var document = _fixture.Build<DocumentEntity>()
                .Without(x => x.Claims)
                .Create();
            DatabaseContext.Documents.Add(document);
            DatabaseContext.SaveChanges();

            var entity = _fixture.Build<ClaimEntity>()
                .Without(x => x.Document)
                .With(x => x.DocumentId, document.Id)
                .Create();
            DatabaseContext.Claims.Add(entity);
            DatabaseContext.SaveChanges();
            var domain = entity.ToDomain();

            domain.Id.Should().Be(entity.Id);
            domain.CreatedAt.Should().Be(entity.CreatedAt);
            domain.Document.Should().BeEquivalentTo(document.ToDomain());
            domain.ApiCreatedBy.Should().Be(entity.ApiCreatedBy);
            domain.UserCreatedBy.Should().Be(entity.UserCreatedBy);
            domain.ServiceAreaCreatedBy.Should().Be(entity.ServiceAreaCreatedBy);
            domain.RetentionExpiresAt.Should().Be(entity.RetentionExpiresAt);
            domain.TargetId.Should().Be(entity.TargetId);
        }

        [Test]
        public void CanMapUnsavedClaimEntityToDomainModel()
        {
            var document = _fixture.Build<DocumentEntity>()
                .Without(x => x.Claims)
                .Create();
            DatabaseContext.Documents.Add(document);
            DatabaseContext.SaveChanges();

            var entity = _fixture.Build<ClaimEntity>()
                .Without(x => x.Document)
                .With(x => x.DocumentId, document.Id)
                .Create();
            var domain = entity.ToDomain();

            domain.Id.Should().Be(entity.Id);
            domain.CreatedAt.Should().Be(entity.CreatedAt);
            domain.Document.Should().BeNull();
            domain.ApiCreatedBy.Should().Be(entity.ApiCreatedBy);
            domain.UserCreatedBy.Should().Be(entity.UserCreatedBy);
            domain.ServiceAreaCreatedBy.Should().Be(entity.ServiceAreaCreatedBy);
            domain.RetentionExpiresAt.Should().Be(entity.RetentionExpiresAt);
        }
    }
}
