using System;
using System.IO;
using System.Threading.Tasks;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IDownloadDocumentUseCase
    {
        public Tuple<Document, Task<Stream>> Execute(Guid documentId);
    }
}
