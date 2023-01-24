#nullable enable annotations
using System;

namespace DocumentsApi.V1.Boundary.Request
{
    public class PaginatedClaimRequest
    {
        public Guid GroupId { get; set; }
        public int? Limit { get; set; }
        public string? Before { get; set; } = null; // base64URL
        public string? After { get; set; } = null; // base64URL
    }
}
