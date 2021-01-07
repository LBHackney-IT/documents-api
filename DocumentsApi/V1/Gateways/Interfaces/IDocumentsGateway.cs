using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IDocumentsGateway
    {
        public Document CreateDocument(Document request);
        public Claim CreateClaim(Claim request);
    }
}
