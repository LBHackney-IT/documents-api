using System;

namespace DocumentsApi.V1.Boundary.Response
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
    }
}
