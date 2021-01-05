using System;

namespace DocumentsApi.V1.Domain
{
    public class Document
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
    }
}
