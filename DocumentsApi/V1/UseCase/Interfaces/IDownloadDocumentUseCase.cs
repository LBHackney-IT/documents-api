using System;
using System.Threading.Tasks;
using System.IO;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IDownloadDocumentUseCase
    {
        public Tuple<Document, Task<Stream>> Execute(Guid documentId);
    }
}
