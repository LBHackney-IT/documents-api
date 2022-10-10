using System;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Boundary.Response
{
    public class ClaimAndS3UploadPolicyResponse
    {
        public Guid ClaimId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DocumentResponse Document { get; set; }
        public string ServiceAreaCreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApiCreatedBy { get; set; }
        public DateTime RetentionExpiresAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public Guid? TargetId { get; set; } = null;
        public S3UploadPolicy S3UploadPolicy { get; set; }
        public string TargetType { get; set; }

    }
}
