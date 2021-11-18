using System;

namespace DocumentsApi.V1.Boundary.Request
{
    public class ClaimAndUploadDocumentRequest
    {
        public string ServiceAreaCreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApiCreatedBy { get; set; }
        public DateTime RetentionExpiresAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public string Base64Document { get; set; }
    }
}
