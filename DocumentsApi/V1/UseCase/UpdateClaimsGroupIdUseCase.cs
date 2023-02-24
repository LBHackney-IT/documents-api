using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.UseCase.Interfaces;

namespace DocumentsApi.V1.UseCase
{
    public class UpdateClaimsGroupIdUseCase : IUpdateClaimsGroupIdUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;

        public UpdateClaimsGroupIdUseCase(IDocumentsGateway documentsGateway)
        {
            _documentsGateway = documentsGateway;
        }

        public List<ClaimResponse> Execute(ClaimsUpdateRequest request)
        {
            var claimsResponse = new List<ClaimResponse>();
            var claims = _documentsGateway.UpdateClaimsGroupId(request.OldGroupId, request.NewGroupId);
            foreach (var claim in claims)
            {
                claimsResponse.Add(claim.ToResponse());
            }
            return claimsResponse;
        }
    }
}
