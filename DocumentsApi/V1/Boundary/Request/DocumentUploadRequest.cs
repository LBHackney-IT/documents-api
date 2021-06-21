using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsApi.V1.Boundary.Request
{
    public class DocumentUploadRequest
    {
        [FromRoute]
        public Guid Id { get; set; }
        public IFormFile Document { get; set; }
    }
}
