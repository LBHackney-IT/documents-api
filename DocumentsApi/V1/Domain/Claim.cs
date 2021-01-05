using System;

namespace DocumentsApi.V1.Domain
{
    public class Claim
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Document Document { get; set; }
        public string ServiceAreaCreatedBy { get; set; }
        public string UserCreatedBy { get; set; }
        public string ApiCreatedBy { get; set; }
        public DateTime RetentionExpiresAt { get; set; }
    }
}
