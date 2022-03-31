using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Domain;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static DocumentResponse ToResponse(this Document domain)
        {
            return new DocumentResponse
            {
                CreatedAt = domain.CreatedAt,
                FileSize = domain.FileSize,
                FileType = domain.FileType,
                Id = domain.Id,
                UploadedAt = domain.UploadedAt
            };
        }

        public static ClaimResponse ToResponse(this Claim domain)
        {
            return new ClaimResponse
            {
                ApiCreatedBy = domain.ApiCreatedBy,
                CreatedAt = domain.CreatedAt,
                Document = domain.Document.ToResponse(),
                ServiceAreaCreatedBy = domain.ServiceAreaCreatedBy,
                Id = domain.Id,
                RetentionExpiresAt = domain.RetentionExpiresAt,
                UserCreatedBy = domain.UserCreatedBy,
                ValidUntil = domain.ValidUntil
            };
        }

        // Suppress CA1054 because GetPreSignedURL returns a string, not an Uri
        [SuppressMessage("ReSharper", "CA1054")]
        public static GetClaimAndPreSignedDownloadUrlResponse ToClaimAndPreSignedDownloadUrlResponse(this Claim claim, string preSignedDownloadUrl)
        {
            return new GetClaimAndPreSignedDownloadUrlResponse
            {
                ApiCreatedBy = claim.ApiCreatedBy,
                CreatedAt = claim.CreatedAt,
                Document = claim.Document.ToResponse(),
                ServiceAreaCreatedBy = claim.ServiceAreaCreatedBy,
                ClaimId = claim.Id,
                RetentionExpiresAt = claim.RetentionExpiresAt,
                UserCreatedBy = claim.UserCreatedBy,
                ValidUntil = claim.ValidUntil,
                PreSignedDownloadUrl = preSignedDownloadUrl
            };
        }

        public static CreateClaimAndS3UploadPolicyResponse ToClaimAndS3UploadPolicyResponse(this Claim claim, S3UploadPolicy s3UploadPolicy)
        {
            return new CreateClaimAndS3UploadPolicyResponse
            {
                ApiCreatedBy = claim.ApiCreatedBy,
                CreatedAt = claim.CreatedAt,
                Document = claim.Document.ToResponse(),
                ServiceAreaCreatedBy = claim.ServiceAreaCreatedBy,
                ClaimId = claim.Id,
                RetentionExpiresAt = claim.RetentionExpiresAt,
                UserCreatedBy = claim.UserCreatedBy,
                ValidUntil = claim.ValidUntil,
                S3UploadPolicy = s3UploadPolicy
            };
        }
    }
}
