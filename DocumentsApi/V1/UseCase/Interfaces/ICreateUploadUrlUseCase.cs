using System;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface ICreateUploadUrlUseCase
    {
        public Task<S3UploadPolicy> Execute(Guid documentId);
    }
}
