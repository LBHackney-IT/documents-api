using System;
using System.Linq;
using DocumentsApi.V1.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace DocumentsApi.V1.Infrastructure
{

    public class DocumentsContext : DbContext
    {
        private readonly AppOptions _appOptions;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(_appOptions.DatabaseConnectionString).AddXRayInterceptor();

        public DocumentsContext(DbContextOptions options, AppOptions appAppOptions) : base(options)
        {
            _appOptions = appAppOptions;
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
                if (entity.CreatedAt == default) entity.CreatedAt = DateTime.UtcNow;
                if (entity.Id == default) entity.Id = Guid.NewGuid();
            }

            return base.SaveChanges();
        }

    }
}
