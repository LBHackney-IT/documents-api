using System;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IDownloadDocumentWithUrlUseCase
    {
        public string Execute(string documentId);
    }
}
