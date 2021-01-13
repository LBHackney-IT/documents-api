using System;
using System.Collections.Generic;

namespace DocumentsApi.V1.Domain
{
    public class S3UploadPolicy
    {
        public Uri Url { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
