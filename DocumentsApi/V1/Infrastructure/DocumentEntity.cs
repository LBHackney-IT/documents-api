using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DocumentsApi.V1.Infrastructure.Interfaces;

namespace DocumentsApi.V1.Infrastructure
{
    [Table("documents")]
    public class DocumentEntity : IEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("file_size")]
        public long FileSize { get; set; }

        [Column("file_type")]
        public string FileType { get; set; }

        [Column("uploaded_at")]
        public DateTime? UploadedAt { get; set; }

        public virtual ICollection<ClaimEntity> Claims { get; set; }
    }
}
