using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Domain;

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

        //TODO: Amend when removing old download path
        public static ClaimAndDocumentResponse ToClaimAndDocumentResponse(this ClaimResponse claim, string base64Document)
        {
            return new ClaimAndDocumentResponse
            {
                ApiCreatedBy = claim.ApiCreatedBy,
                CreatedAt = claim.CreatedAt,
                Document = claim.Document,
                ServiceAreaCreatedBy = claim.ServiceAreaCreatedBy,
                ClaimId = claim.Id,
                RetentionExpiresAt = claim.RetentionExpiresAt,
                UserCreatedBy = claim.UserCreatedBy,
                ValidUntil = claim.ValidUntil,
                Base64Document = base64Document
            };
        }

        public static CreateClaimAndS3UploadPolicyResponse ToClaimAndS3PolicyResponse(this Claim claim, S3UploadPolicy s3UploadPolicy)
        {
            return new CreateClaimAndS3UploadPolicyResponse
            {
                ApiCreatedBy = claim.ApiCreatedBy,
                CreatedAt = claim.CreatedAt,
                Document = claim.Document,
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
