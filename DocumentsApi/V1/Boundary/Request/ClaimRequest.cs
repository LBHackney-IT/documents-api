using System;

namespace DocumentsApi.V1.Boundary.Request
{
    public class ClaimRequest
    {
        public string ServiceAreaCreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApiCreatedBy { get; set; }
        public DateTime RetentionExpiresAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public Guid? GroupId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentDescription { get; set; }
        public string TargetType { get; set; }
    }
}
