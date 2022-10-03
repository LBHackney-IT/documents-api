using System.Linq;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Validators;
using DocumentsApi.V1.Gateways.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DocumentsApi.V1.UseCase
{
    public class CreateClaimAndS3UploadPolicyUseCase : ICreateClaimAndS3UploadPolicyUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;
        private readonly IS3Gateway _s3Gateway;
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

        public async Task<ClaimAndS3UploadPolicyResponse> ExecuteAsync(ClaimRequest request)
        {
            var validation = new ClaimRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogError("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
                throw new BadRequestException(validation);
            }

            var claim = request.ToDomain();
            var createdClaim = _documentsGateway.CreateClaim(claim);
            var s3UploadPolicy = await _s3Gateway.GenerateUploadPolicy(createdClaim.Document).ConfigureAwait(true);

            return createdClaim.ToClaimAndS3UploadPolicyResponse(s3UploadPolicy);
        }
    }
}
