using System;

namespace DocumentsApi.V1.Boundary.Response.Exceptions
{
    public class DocumentUploadException: Exception
    {
        public DocumentUploadException(string message) : base(message) { }
    }
}
