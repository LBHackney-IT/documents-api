using System;
using System.Linq;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Validators;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class CreateClaimUseCase : ICreateClaimUseCase
    {
        private IDocumentsGateway _documentsGateway;
        private readonly ILogger<CreateClaimUseCase> _logger;

        public CreateClaimUseCase(IDocumentsGateway documentsGateway, ILogger<CreateClaimUseCase> logger)
        {
            _documentsGateway = documentsGateway;
            _logger = logger;
        }

        public ClaimResponse Execute(ClaimRequest request)
        {
            var validation = new ClaimRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogError("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
                throw new BadRequestException(validation);
            }

            var claim = BuildClaimRequest(request);

            var created = _documentsGateway.CreateClaim(claim);

            return created.ToResponse();
        }

        private static Claim BuildClaimRequest(ClaimRequest request)
        {
            return new Claim
            {
                ApiCreatedBy = request.ApiCreatedBy,
                ServiceAreaCreatedBy = request.ServiceAreaCreatedBy,
                UserCreatedBy = request.UserCreatedBy,
                RetentionExpiresAt = request.RetentionExpiresAt,
                ValidUntil = request.ValidUntil,
                TargetId = request.TargetId,
                Document = new Document
                {
                    Name = request.DocumentName,
                    Description = request.DocumentDescription
                }
            };
        }
    }
}
