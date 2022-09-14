using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Factories;
using System.Collections.Generic;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using System;

namespace DocumentsApi.V1.UseCase
{
    public class GetClaimsByTargetIdUseCase : IGetClaimsByTargetIdUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;

        public GetClaimsByTargetIdUseCase(IDocumentsGateway documentsGateway)
        {
            _documentsGateway = documentsGateway;
        }

        public Dictionary<string, List<ClaimResponse>> Execute(Guid targetId)
        {
            var claims = _documentsGateway.FindClaimsByTargetId(targetId);
            
            if (claims == null)
            {
                throw new NotFoundException($"No claims have been found for target ID: {targetId}");
            }

            var result = new Dictionary<string, List<ClaimResponse>>()
            {
                {"claims", new List<ClaimResponse>()}
            };
            
            foreach(var claim in claims)
            {
                result["claims"].Add(claim.ToResponse());
            }
            return result;
        }
    }
}