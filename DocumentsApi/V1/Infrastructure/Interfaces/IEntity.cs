using System;

namespace DocumentsApi.V1.Infrastructure.Interfaces
{
    public interface IEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
