using System;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IGeneratePreSignedDownloadUrlUseCase
    {
        public string Execute(Guid claimId);
    }
}
