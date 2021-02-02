using System;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IDocumentsGateway
    {
        public Document SaveDocument(Document request);
        public Claim CreateClaim(Claim request);
        public Document FindDocument(Guid id);
        public Claim FindClaim(Guid id);
    }
}
