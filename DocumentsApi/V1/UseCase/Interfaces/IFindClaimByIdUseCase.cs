using System;
using DocumentsApi.V1.Boundary.Response;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IFindClaimByIdUseCase
    {
        public ClaimResponse Execute(Guid id);
    }
}
