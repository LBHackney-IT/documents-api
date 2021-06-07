using System;
using System.Threading.Tasks;
using System.IO;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IDownloadDocumentUseCase
    {
        public Task<Stream> Execute(Guid documentId);
    }
}
