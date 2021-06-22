using DocumentsApi.V1.Boundary.Request;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IUploadDocumentUseCase
    {
        public void Execute(DocumentUploadRequest request);
    }
}
