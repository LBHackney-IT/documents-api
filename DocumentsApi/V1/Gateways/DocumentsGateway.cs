using System;
using System.Linq;
using System.Collections.Generic;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DocumentsApi.V1.Gateways
{
    public class DocumentsGateway : IDocumentsGateway
    {
        private readonly DocumentsContext _databaseContext;

        public DocumentsGateway(DocumentsContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Document SaveDocument(Document document)
        {
            var entity = document.ToEntity();

            _databaseContext.Documents.Add(entity);
            if (document.Id != default) _databaseContext.Entry(entity).State = EntityState.Modified;
            _databaseContext.SaveChanges();

            return entity.ToDomain();
        }

        public Claim CreateClaim(Claim claim)
        {
            var entity = claim.ToEntity();

            _databaseContext.Claims.Add(entity);
            if (claim.Id != default) _databaseContext.Entry(entity).State = EntityState.Modified;
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
            var document = _databaseContext.Documents.Find(entity.DocumentId);
            entity.Document = document;

            _databaseContext.Entry(entity).State = EntityState.Detached;
            return entity.ToDomain();
        }

        public List<Claim> FindPaginatedClaimsByGroupId(Guid groupId, int? limit, Guid? cursor, bool? isNextPage)
        {
            IQueryable<ClaimEntity> entities;

            if (cursor == null)
            {
                entities = _databaseContext.Claims
                    .Where(claimEntity => claimEntity.GroupId == groupId)
                    .Include(claimEntity => claimEntity.Document)
                    .OrderByDescending(claimEntity => claimEntity.CreatedAt)
                    .ThenBy(claimEntity => claimEntity.Id);
            }
            else
            {
                if (isNextPage == true)
                    if (limit > 0)
                    {
                        entities = _databaseContext.Claims
                            .Where(
                                claimEntity => claimEntity.GroupId == groupId &&
                                               claimEntity.CreatedAt < _databaseContext.Claims.Find(cursor).CreatedAt ||
                                               (claimEntity.CreatedAt == _databaseContext.Claims.Find(cursor).CreatedAt &&
                                                claimEntity.Id.CompareTo(_databaseContext.Claims.Find(cursor).Id) > 0))
                            .Include(claimEntity => claimEntity.Document)
                            .OrderByDescending(claimEntity => claimEntity.CreatedAt)
                            .ThenBy(claimEntity => claimEntity.Id)
                            .Take(limit ?? default(int));
                    }
                    else
                    {
                        entities = _databaseContext.Claims
                            .Where(
                                claimEntity => claimEntity.GroupId == groupId &&
                                               claimEntity.CreatedAt < _databaseContext.Claims.Find(cursor).CreatedAt ||
                                               (claimEntity.CreatedAt == _databaseContext.Claims.Find(cursor).CreatedAt &&
                                                claimEntity.Id.CompareTo(_databaseContext.Claims.Find(cursor).Id) > 0))
                            .Include(claimEntity => claimEntity.Document)
                            .OrderByDescending(claimEntity => claimEntity.CreatedAt)
                            .ThenBy(claimEntity => claimEntity.Id);
                    }
                else
                {
                    if (limit > 0)
                    {
                        entities = _databaseContext.Claims
                            .Where(
                                claimEntity => claimEntity.GroupId == groupId &&
                                    claimEntity.CreatedAt > _databaseContext.Claims.Find(cursor).CreatedAt ||
                                    (claimEntity.CreatedAt == _databaseContext.Claims.Find(cursor).CreatedAt &&
                                    claimEntity.Id.CompareTo(_databaseContext.Claims.Find(cursor).Id) < 0))
                            .Include(claimEntity => claimEntity.Document)
                            .OrderBy(claimEntity => claimEntity.CreatedAt)
                            .Take(limit ?? default(int))
                            .OrderByDescending(claimEntity => claimEntity.CreatedAt)
                            .ThenBy(claimEntity => claimEntity.Id);
                    }
                    else
                    {
                        entities = _databaseContext.Claims
                            .Where(
                                claimEntity => claimEntity.GroupId == groupId &&
                                    claimEntity.CreatedAt > _databaseContext.Claims.Find(cursor).CreatedAt ||
                                    (claimEntity.CreatedAt == _databaseContext.Claims.Find(cursor).CreatedAt &&
                                    claimEntity.Id.CompareTo(_databaseContext.Claims.Find(cursor).Id) < 0))
                            .Include(claimEntity => claimEntity.Document)
                            .OrderBy(claimEntity => claimEntity.CreatedAt)
                            .OrderByDescending(claimEntity => claimEntity.CreatedAt)
                            .ThenBy(claimEntity => claimEntity.Id);
                    }
                }
            }

            var claims = new List<Claim>();
            foreach (var entity in entities)
            {
                claims.Add(entity.ToDomain());
            }
            return claims;
        }

        public Claim SaveClaim(Claim request)
        {
            var entity = request.ToEntity();
            _databaseContext.Entry(entity).State = EntityState.Modified;
            _databaseContext.SaveChanges();

            return entity.ToDomain();
        }
    }
}
