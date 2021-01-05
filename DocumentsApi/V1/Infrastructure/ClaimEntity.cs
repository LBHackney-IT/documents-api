using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentsApi.V1.Infrastructure.Interfaces;

namespace DocumentsApi.V1.Infrastructure
{
    [Table("claims")]
    public class ClaimEntity : IEntity
    {
        [Required]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("document_id")]
        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }

        [ForeignKey("DocumentId")]
        public virtual DocumentEntity Document { get; set; }

        [Column("service_area_created_by")]
        public string ServiceAreaCreatedBy { get; set; }

        [Column("user_created_by")]
        public string UserCreatedBy { get; set; }

        [Required]
        [Column("api_created_by")]
        public string ApiCreatedBy { get; set; }

        [Required]
        [Column("retention_expires_at")]
        public DateTime RetentionExpiresAt { get; set; }
    }
}
