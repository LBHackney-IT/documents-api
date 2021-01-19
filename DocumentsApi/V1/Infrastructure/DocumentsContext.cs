using System;
using System.Linq;
using DocumentsApi.V1.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace DocumentsApi.V1.Infrastructure
{

    public class DocumentsContext : DbContext
    {
        public DocumentsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DocumentEntity> Documents { get; set; }
        public DbSet<ClaimEntity> Claims { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IEntity && (e.State == EntityState.Added));

            foreach (var entityEntry in entries)
            {
                var entity = ((IEntity) entityEntry.Entity);
                if (entity.CreatedAt == null) entity.CreatedAt = DateTime.Now;
                if (entity.Id == null) entity.Id = Guid.NewGuid();
            }

            return base.SaveChanges();
        }

    }
}
