using System;
using DocumentsApi.V1.Boundary.Response;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IGetClaimAndDocumentUseCase
    {
        public ClaimAndDocumentResponse Execute(Guid claimId);
    }
}
