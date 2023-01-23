using System;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.Boundary.Response
{
    // Suppress CA1055 because GetPreSignedURL returns a string, not an Uri
    [SuppressMessage("ReSharper", "CA1056")]
    public class ClaimAndPreSignedDownloadUrlResponse
    {
        public Guid ClaimId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DocumentResponse Document { get; set; }
        public string ServiceAreaCreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApiCreatedBy { get; set; }
        public DateTime RetentionExpiresAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public Guid? GroupId { get; set; } = null;
        public string PreSignedDownloadUrl { get; set; }
        public string TargetType { get; set; }
    }
}
