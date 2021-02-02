using System;
using System.Linq;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DocumentsApi.V1.Gateways
{
    public class DocumentsGateway : IDocumentsGateway
    {
        private readonly DocumentsContext _databaseContext;

        public DocumentsGateway(DocumentsContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Document SaveDocument(Document request)
        {
            var entity = request.ToEntity();

            _databaseContext.Documents.Add(entity);
            if (request.Id != default) _databaseContext.Entry(entity).State = EntityState.Modified;
            _databaseContext.SaveChanges();

            return entity.ToDomain();
        }

        public Claim CreateClaim(Claim request)
        {
            var entity = request.ToEntity();

            _databaseContext.Claims.Add(entity);
            if (request.Id != default) _databaseContext.Entry(entity).State = EntityState.Modified;
            _databaseContext.SaveChanges();

            return entity.ToDomain();
        }

        public Document FindDocument(Guid id)
        {
            var entity = _databaseContext.Documents.Find(id);
            if (entity == null) return null;

            _databaseContext.Entry(entity).State = EntityState.Detached;
            return entity.ToDomain();
        }

        public Claim FindClaim(Guid id)
        {
            var entity = _databaseContext.Claims.Find(id);
            if (entity == null) return null;

            _databaseContext.Entry(entity).State = EntityState.Detached;
            return entity.ToDomain();
        }
    }
}
