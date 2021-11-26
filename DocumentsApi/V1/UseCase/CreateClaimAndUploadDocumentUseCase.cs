using System.Linq;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.UseCase.Interfaces;
using DocumentsApi.V1.Validators;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class CreateClaimAndUploadDocumentUseCase : ICreateClaimAndUploadDocumentUseCase
    {
        private ICreateClaimUseCase _createClaimUseCase;
        private IUploadDocumentUseCase _uploadDocumentUseCase;
        private readonly ILogger<CreateClaimAndUploadDocumentUseCase> _logger;

        public CreateClaimAndUploadDocumentUseCase(ICreateClaimUseCase createClaimUseCase, IUploadDocumentUseCase uploadDocumentUseCase, ILogger<CreateClaimAndUploadDocumentUseCase> logger)
        {
            _createClaimUseCase = createClaimUseCase;
            _uploadDocumentUseCase = uploadDocumentUseCase;
            _logger = logger;
        }

        public ClaimAndDocumentResponse Execute(ClaimAndUploadDocumentRequest request)
        {
            var validation = new ClaimAndUploadDocumentRequestValidator().Validate(request);
            if (!validation.IsValid)
            {
                _logger.LogError("VALIDATION: {0}", validation.Errors.First().ErrorMessage);
                throw new BadRequestException(validation);
            }
            var claimRequest = BuildClaimRequest(request);
            var documentUploadRequest = BuildDocumentUploadRequest(request);
            var claim = _createClaimUseCase.Execute(claimRequest);
            _uploadDocumentUseCase.Execute(claim.Document.Id, documentUploadRequest);

            return claim.ToClaimAndDocumentResponse(request.Base64Document);
        }

        private static ClaimRequest BuildClaimRequest(ClaimAndUploadDocumentRequest request)
        {
            return new ClaimRequest
            {
                ApiCreatedBy = request.ApiCreatedBy,
                ServiceAreaCreatedBy = request.ServiceAreaCreatedBy,
                UserCreatedBy = request.UserCreatedBy,
                RetentionExpiresAt = request.RetentionExpiresAt,
                ValidUntil = request.ValidUntil,
            };
        }

        private static DocumentUploadRequest BuildDocumentUploadRequest(ClaimAndUploadDocumentRequest request)
        {
            return new DocumentUploadRequest
            {
                Base64Document = request.Base64Document
            };
        }
    }
}
