using System;
using System.Linq;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Validators;

namespace DocumentsApi.V1.UseCase
{
    public class CreateClaimUseCase : ICreateClaimUseCase
    {
        private IDocumentsGateway _documentsGateway;

        public CreateClaimUseCase(IDocumentsGateway documentsGateway)
        {
            _documentsGateway = documentsGateway;
        }

        public ClaimResponse Execute(ClaimRequest request)
        {
            var validation = new ClaimRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                Console.WriteLine("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
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
                // TODO: Support creating claims for existing documents
                Document = new Document()
            };
        }
    }
}
