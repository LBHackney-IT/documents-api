using System;

namespace DocumentsApi.V1.Domain
{
    public class Document
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public DateTime? UploadedAt { get; set; }

        public bool Uploaded => UploadedAt != null;
    }
}
