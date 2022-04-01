using System;
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
        public void CanMapAClaimAndS3UploadPolicyToResponse()
        {
            var document = _fixture.Create<Document>();
            var claim = _fixture.Build<Claim>()
                .With(x => x.Document, document)
                .Create();
            var s3UploadPolicy = _fixture.Build<S3UploadPolicy>().Create();

            var response = claim.ToClaimAndS3UploadPolicyResponse(s3UploadPolicy);
            CreateClaimAndS3UploadPolicyResponse expected = new CreateClaimAndS3UploadPolicyResponse
            {
                ClaimId = response.ClaimId,
                CreatedAt = response.CreatedAt,
                Document = response.Document,
                ServiceAreaCreatedBy = response.ServiceAreaCreatedBy,
                UserCreatedBy = response.UserCreatedBy,
                ApiCreatedBy = response.ApiCreatedBy,
                RetentionExpiresAt = response.RetentionExpiresAt,
                ValidUntil = response.ValidUntil,
                S3UploadPolicy = response.S3UploadPolicy
            };

            response.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CanMapAGetClaimAndPreSignedDownloadUrlToResponse()
        {
            var document = _fixture.Create<Document>();
            var claim = _fixture.Build<Claim>()
                .With(x => x.Document, document)
                .Create();
            var s3preSignedDownloadUrl = new String("www.awsS3DownloadUrl.com");

            var response = claim.ToClaimAndPreSignedDownloadUrlResponse(s3preSignedDownloadUrl);
            var expected = new ClaimAndPreSignedDownloadUrlResponse
            {
                ClaimId = response.ClaimId,
                CreatedAt = response.CreatedAt,
                Document = response.Document,
                ServiceAreaCreatedBy = response.ServiceAreaCreatedBy,
                UserCreatedBy = response.UserCreatedBy,
                ApiCreatedBy = response.ApiCreatedBy,
                RetentionExpiresAt = response.RetentionExpiresAt,
                ValidUntil = response.ValidUntil,
                PreSignedDownloadUrl = s3preSignedDownloadUrl
            };

            response.Should().BeEquivalentTo(expected);
        }
    }
}
