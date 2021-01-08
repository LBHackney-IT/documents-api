using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTests
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void CanMapDocumentDomainToEntityModel()
        {
            var domain = _fixture.Create<Document>();
            var entity = domain.ToEntity();

            domain.Id.Should().Be(entity.Id);
            domain.CreatedAt.Should().Be(entity.CreatedAt);
            domain.FileSize.Should().Be(entity.FileSize);
            domain.FileType.Should().Be(entity.FileType);
        }

        [Test]
        public void CanMapClaimDomainToEntityModel()
        {
            var domain = _fixture.Build<Claim>()
                .With(x => x.Document, new Document())
                .Create();
            var entity = domain.ToEntity();

            domain.Id.Should().Be(entity.Id);
            domain.CreatedAt.Should().Be(entity.CreatedAt);
            domain.Document.Should().BeEquivalentTo(new DocumentEntity(), opt => opt.ExcludingMissingMembers());
            domain.ApiCreatedBy.Should().Be(entity.ApiCreatedBy);
            domain.UserCreatedBy.Should().Be(entity.UserCreatedBy);
            domain.ServiceAreaCreatedBy.Should().Be(entity.ServiceAreaCreatedBy);
            domain.RetentionExpiresAt.Should().Be(entity.RetentionExpiresAt);
        }
    }
}
