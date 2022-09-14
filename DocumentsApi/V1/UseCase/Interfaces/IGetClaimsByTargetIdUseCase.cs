using System;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IGetClaimsByTargetIdUseCase
    {
        public Dictionary<string, List<ClaimResponse>> Execute(Guid targetId);
    }
}
