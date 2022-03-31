using System;
using DocumentsApi.V1.Boundary.Response;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IGetClaimAndPreSignedDownloadUrlUseCase
    {
        public GetClaimAndPreSignedDownloadUrlResponse Execute(Guid claimId);
    }
}
