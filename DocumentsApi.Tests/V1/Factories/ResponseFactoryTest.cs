using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using FluentAssertions;
using NUnit.Framework;
using DocumentsApi.V1.Boundary.Response;

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

            response.Should().BeEquivalentTo(claim, opt => opt.Excluding(x => x.Document.Uploaded).Excluding(x => x.Expired));
        }

        [Test]
        public void CanMapAClaimAndBase64DocumentToResponse()
        {
            var document = _fixture.Create<DocumentResponse>();
            var claim = _fixture.Build<ClaimResponse>()
                .With(x => x.Document, document)
                .Create();
            var base64Document = "base-64-document";

            var response = claim.ToClaimAndDocumentResponse(base64Document);
            ClaimAndDocumentResponse expected = new ClaimAndDocumentResponse
            {
                ClaimId = response.ClaimId,
                CreatedAt = response.CreatedAt,
                Document = response.Document,
                ServiceAreaCreatedBy = response.ServiceAreaCreatedBy,
                UserCreatedBy = response.UserCreatedBy,
                ApiCreatedBy = response.ApiCreatedBy,
                RetentionExpiresAt = response.RetentionExpiresAt,
                ValidUntil = response.ValidUntil,
                Base64Document = response.Base64Document
            };

            response.Should().BeEquivalentTo(expected);
        }
    }
}
