using System;
using System.Threading.Tasks;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IDownloadDocumentUseCase
    {
        public string Execute(Guid documentId);
    }
}
