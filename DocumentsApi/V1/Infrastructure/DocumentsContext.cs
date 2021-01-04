using System;
using System.Linq;
using DocumentsApi.V1.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DocumentsApi.V1.Infrastructure
{

    public class DocumentsContext : DbContext
    {
        public DocumentsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DatabaseEntity> DatabaseEntities { get; set; }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IEntity && (e.State == EntityState.Added));

            foreach (var entityEntry in entries)
            {
                var entity = ((IEntity) entityEntry.Entity);
                entity.CreatedAt = DateTime.Now;
                entity.Id = Guid.NewGuid();
            }

            return base.SaveChanges();
        }

    }
}
