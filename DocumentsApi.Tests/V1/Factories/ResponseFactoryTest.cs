using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void CanMapAClaimDomainModelToResponse()
        {
            var document = _fixture.Create<Document>();
            var claim = _fixture.Build<Claim>()
                .With(x => x.Document, document)
                .Create();

            var response = claim.ToResponse();

            response.Should().BeEquivalentTo(claim);
        }
    }
}
