using System;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.Gateways.Interfaces
{
    public interface IS3Gateway
    {
        public Uri GenerateUploadUrl(Document document);
    }
}
