using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Infrastructure;

namespace DocumentsApi.V1.Factories
{
    public static class DomainFactory
    {
        public static Document ToDomain(this DocumentEntity entity)
        {
            return new Document
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Name = entity.Name,
                Description = entity.Description,
                FileSize = entity.FileSize,
                FileType = entity.FileType,
                UploadedAt = entity.UploadedAt
            };
        }
        public static Claim ToDomain(this ClaimEntity entity)
        {
            return new Claim
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Document = entity.Document?.ToDomain(),
                ApiCreatedBy = entity.ApiCreatedBy,
                UserCreatedBy = entity.UserCreatedBy,
                ServiceAreaCreatedBy = entity.ServiceAreaCreatedBy,
                RetentionExpiresAt = entity.RetentionExpiresAt,
                ValidUntil = entity.ValidUntil,
                GroupId = entity.GroupId,
                TargetType = entity.TargetType
            };
        }

        public static Claim ToDomain(this ClaimRequest request)
        {
            return new Claim
            {
                ApiCreatedBy = request.ApiCreatedBy,
                ServiceAreaCreatedBy = request.ServiceAreaCreatedBy,
                UserCreatedBy = request.UserCreatedBy,
                RetentionExpiresAt = request.RetentionExpiresAt,
                ValidUntil = request.ValidUntil,
                GroupId = request.GroupId,
                TargetType = request.TargetType,
                Document = new Document()
                {
                    Name = request.DocumentName,
                    Description = request.DocumentDescription
                }
            };
        }

    }
}
