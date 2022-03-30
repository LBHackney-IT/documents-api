using System.Linq;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Validators;
using DocumentsApi.V1.Gateways.Interfaces;
using Microsoft.Extensions.Logging;
using DocumentsApi.V1.Domain;
using System.Threading.Tasks;

namespace DocumentsApi.V1.UseCase
{
    public class CreateClaimAndS3UploadPolicyUseCase : ICreateClaimAndS3UploadPolicyUseCase
    {
        private IDocumentsGateway _documentsGateway;
        private IS3Gateway _s3Gateway;
        private readonly ILogger<CreateClaimAndS3UploadPolicyUseCase> _logger;

        public CreateClaimAndS3UploadPolicyUseCase(
        IDocumentsGateway documentsGateway,
        IS3Gateway s3Gateway,
        ILogger<CreateClaimAndS3UploadPolicyUseCase> logger)
        {
            _documentsGateway = documentsGateway;
            _s3Gateway = s3Gateway;
            _logger = logger;
        }

        public async Task<CreateClaimAndS3UploadPolicyResponse> ExecuteAsync(ClaimRequest request)
        {
            var validation = new ClaimRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogError("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
                throw new BadRequestException(validation);
            }

            var claim = BuildClaimRequest(request);
            var createdClaim = _documentsGateway.CreateClaim(claim);
            var s3UploadPolicy = await _s3Gateway.GenerateUploadPolicy(createdClaim.Document).ConfigureAwait(true);

            return createdClaim.ToClaimAndS3PolicyResponse(s3UploadPolicy);
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
                Document = new Document()
            };
        }
    }
}
