using System;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;

namespace DocumentsApi.V1.UseCase
{
    public class UpdateClaimStateUseCase : IUpdateClaimStateUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;

        public UpdateClaimStateUseCase(IDocumentsGateway documentsGateway)
        {
            _documentsGateway = documentsGateway;
        }

        public ClaimResponse Execute(Guid id, ClaimUpdateRequest request)
        {
            var found = _documentsGateway.FindClaim(id);

            if (found == null)
            {
                throw new NotFoundException($"Could not find Claim with ID: {id}");
            }

            if (request == null)
            {
                throw new BadRequestException($"Cannot update Claim with ID: {id} because of invalid request.");
            }

            found.ValidUntil = request.ValidUntil;
            _documentsGateway.SaveClaim(found);

            return found.ToResponse();
        }
    }
}
