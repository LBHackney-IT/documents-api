using System;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IGetClaimsByGroupIdUseCase
    {
        public PaginatedClaimResponse Execute(PaginatedClaimRequest request);
    }
}
