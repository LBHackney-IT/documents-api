using System;
using System.Collections.Generic;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IDocumentsGateway
    {
        public Document SaveDocument(Document request);
        public Claim CreateClaim(Claim request);
        public Document FindDocument(Guid id);
        public Claim FindClaim(Guid id);
        public List<Claim> FindClaimsByTargetId(Guid targetId, int limit);
        public List<Claim> FindPaginatedClaimsByTargetId(Guid targetId, int limit, Guid? cursor, bool? isNextPage);
        public Claim SaveClaim(Claim request);
    }
}
