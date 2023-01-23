using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Infrastructure;

namespace DocumentsApi.V1.Factories
{
    public static class EntityFactory
    {
        public static DocumentEntity ToEntity(this Document domain)
        {
            return new DocumentEntity
            {
                Id = domain.Id,
                CreatedAt = domain.CreatedAt,
                Name = domain.Name,
                Description = domain.Description,
                FileSize = domain.FileSize,
                FileType = domain.FileType,
                UploadedAt = domain.UploadedAt
            };
        }

        public static ClaimEntity ToEntity(this Claim domain)
        {
            return new ClaimEntity
            {
                Id = domain.Id,
                CreatedAt = domain.CreatedAt,
                Document = domain.Document?.ToEntity(),
                DocumentId = domain.Document.Id,
                ApiCreatedBy = domain.ApiCreatedBy,
                UserCreatedBy = domain.UserCreatedBy,
                ServiceAreaCreatedBy = domain.ServiceAreaCreatedBy,
                RetentionExpiresAt = domain.RetentionExpiresAt,
                ValidUntil = domain.ValidUntil,
                GroupId = domain.GroupId,
                TargetType = domain.TargetType
            };
        }
    }
}
