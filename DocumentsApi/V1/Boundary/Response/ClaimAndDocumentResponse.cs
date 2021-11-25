using System;

namespace DocumentsApi.V1.Boundary.Response
{
    public class ClaimAndDocumentResponse
    {
        public Guid ClaimId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DocumentResponse Document { get; set; }
        public string ServiceAreaCreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApiCreatedBy { get; set; }
        public DateTime RetentionExpiresAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public string Base64Document { get; set; }
    }
}
