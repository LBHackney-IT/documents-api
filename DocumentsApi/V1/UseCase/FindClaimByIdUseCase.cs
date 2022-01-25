using System;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;

namespace DocumentsApi.V1.UseCase
{
    public class FindClaimByIdUseCase : IFindClaimByIdUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;

        public FindClaimByIdUseCase(IDocumentsGateway documentsGateway)
        {
            _documentsGateway = documentsGateway;
        }

        public ClaimResponse Execute(Guid id)
        {
            var found = _documentsGateway.FindClaim(id);

            if (found == null)
            {
                throw new NotFoundException($"Could not find Claim with ID {id}");
            }

            return found.ToResponse();
        }
    }
}
