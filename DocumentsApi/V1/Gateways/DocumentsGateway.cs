using System;
using System.Linq;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;

namespace DocumentsApi.V1.Gateways
{
    public class DocumentsGateway : IDocumentsGateway
    {
        private readonly DocumentsContext _databaseContext;

        public DocumentsGateway(DocumentsContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Document CreateDocument(Document request)
        {
            var entity = request.ToEntity();
            _databaseContext.Documents.Add(entity);
            _databaseContext.SaveChanges();

            return entity.ToDomain();
        }

        public Claim CreateClaim(Claim request)
        {
            var entity = request.ToEntity();
            _databaseContext.Claims.Add(entity);
            _databaseContext.SaveChanges();

            return entity.ToDomain();
        }

        public Document FindDocument(Guid id)
        {
            return _databaseContext.Documents.Find(id)?.ToDomain();
        }
    }
}
