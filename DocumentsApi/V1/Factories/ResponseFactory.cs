using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Domain;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace DocumentsApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static DocumentResponse ToResponse(this Document domain)
        {
            return new DocumentResponse
            {
                CreatedAt = domain.CreatedAt,
                Name = domain.Name,
                DocumentDescription = domain.Description,
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
                ValidUntil = domain.ValidUntil,
                TargetId = domain.TargetId,
                TargetType = domain.TargetType
            };
        }

        // Suppress CA1054 because GetPreSignedURL returns a string, not an Uri
        [SuppressMessage("ReSharper", "CA1054")]
        public static ClaimAndPreSignedDownloadUrlResponse ToClaimAndPreSignedDownloadUrlResponse(this Claim claim, string preSignedDownloadUrl)
        {
            return new ClaimAndPreSignedDownloadUrlResponse
            {
                ApiCreatedBy = claim.ApiCreatedBy,
                CreatedAt = claim.CreatedAt,
                Document = claim.Document.ToResponse(),
                ServiceAreaCreatedBy = claim.ServiceAreaCreatedBy,
                ClaimId = claim.Id,
                RetentionExpiresAt = claim.RetentionExpiresAt,
                UserCreatedBy = claim.UserCreatedBy,
                ValidUntil = claim.ValidUntil,
                TargetId = claim.TargetId,
                TargetType = claim.TargetType,
                PreSignedDownloadUrl = preSignedDownloadUrl
            };
        }

        public static ClaimAndS3UploadPolicyResponse ToClaimAndS3UploadPolicyResponse(this Claim claim, S3UploadPolicy s3UploadPolicy)
        {
            return new ClaimAndS3UploadPolicyResponse
            {
                ApiCreatedBy = claim.ApiCreatedBy,
                CreatedAt = claim.CreatedAt,
                Document = claim.Document.ToResponse(),
                ServiceAreaCreatedBy = claim.ServiceAreaCreatedBy,
                ClaimId = claim.Id,
                RetentionExpiresAt = claim.RetentionExpiresAt,
                UserCreatedBy = claim.UserCreatedBy,
                ValidUntil = claim.ValidUntil,
                TargetId = claim.TargetId,
                TargetType = claim.TargetType,
                S3UploadPolicy = s3UploadPolicy
            };
        }

        public static PaginatedClaimResponse ToPaginatedClaimResponse(List<ClaimResponse> claims, string before, string after, bool hasBefore, bool hasAfter)
        {
            return new PaginatedClaimResponse
            {
                Claims = claims,
                Paging = new Paging()
                {
                    Cursors = new Cursors()
                    {
                        Before = before,
                        After = after
                    },
                    HasBefore = hasBefore,
                    HasAfter = hasAfter
                }
            };
        }
    }
}
