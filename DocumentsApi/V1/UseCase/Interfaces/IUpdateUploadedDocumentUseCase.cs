using System.Threading.Tasks;
using Amazon.Lambda.S3Events;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IUpdateUploadedDocumentUseCase
    {
        public Task ExecuteAsync(S3Event s3Event);
    }
}
