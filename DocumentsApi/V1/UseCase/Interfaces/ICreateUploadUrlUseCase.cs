using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface ICreateUploadUrlUseCase
    {
        public UrlResponse Execute(Guid documentId);
    }
}
