using System;

namespace DocumentsApi.V1.Boundary.Request
{
    public class ClaimsUpdateRequest
    {
        public Guid OldGroupId { get; set; }
        public Guid NewGroupId { get; set; }
    }
}
