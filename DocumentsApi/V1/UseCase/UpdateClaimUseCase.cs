using System;
using System.Linq;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Validators;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class UpdateClaimUseCase : IUpdateClaimUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;
        private readonly ILogger<UpdateClaimUseCase> _logger;

        public UpdateClaimUseCase(IDocumentsGateway documentsGateway, ILogger<UpdateClaimUseCase> logger)
        {
            _documentsGateway = documentsGateway;
            _logger = logger;
        }

        public ClaimResponse Execute(Guid id, ClaimUpdateRequest request)
        {
            if (request == null)
            {
                throw new BadRequestException($"Cannot update Claim with ID: {id} because of invalid request.");
            }

            var validation = new ClaimUpdateRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogError("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
                throw new BadRequestException(validation);
            }
            var found = _documentsGateway.FindClaim(id);

            if (found == null)
            {
                throw new NotFoundException($"Could not find Claim with ID: {id}");
            }

            if (request.ValidUntil != null && request.ValidUntil < DateTime.UtcNow)
            {
                throw new BadRequestException("The date cannot be in the past.");
            }

            if (request.GroupId != null)
            {
                found.GroupId = request.GroupId;
            }

            if (request.ValidUntil != null)
            {
                found.ValidUntil = (DateTime) request.ValidUntil;
            }
            _documentsGateway.SaveClaim(found);

            return found.ToResponse();
        }
    }
}
