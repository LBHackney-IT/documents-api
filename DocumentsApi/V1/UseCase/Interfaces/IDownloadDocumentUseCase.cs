using System;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IDownloadDocumentUseCase
    {
        public Tuple<Document, string> Execute(Guid documentId);
    }
}
