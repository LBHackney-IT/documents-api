using System;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Request;

namespace DocumentsApi.V1.UseCase.Interfaces
{
    public interface IUpdateClaimsGroupIdUseCase
    {
        public List<ClaimResponse> Execute(ClaimsUpdateRequest request);
    }
}
