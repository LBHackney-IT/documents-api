using System;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;
using DocumentsApi.V1.Gateways.Interfaces;

namespace DocumentsApi.V1.UseCase
{
    public class GetClaimAndPreSignedDownloadUrlUseCase : IGetClaimAndPreSignedDownloadUrlUseCase
    {
        private readonly IS3Gateway _s3Gateway;
        private readonly IDocumentsGateway _documentsGateway;
        private readonly ILogger<GetClaimAndPreSignedDownloadUrlUseCase> _logger;

        public GetClaimAndPreSignedDownloadUrlUseCase(
            IS3Gateway s3Gateway,
            IDocumentsGateway documentsGateway,
            ILogger<GetClaimAndPreSignedDownloadUrlUseCase> logger)
        {
            _s3Gateway = s3Gateway;
            _documentsGateway = documentsGateway;
            _logger = logger;
        }

        public ClaimAndPreSignedDownloadUrlResponse Execute(Guid claimId)
        {
            var foundClaim = _documentsGateway.FindClaim(claimId);
            if (foundClaim == null)
            {
                throw new NotFoundException($"Could not find Claim with ID {claimId}");
            }

            var preSignedDownloadUrl = _s3Gateway.GeneratePreSignedDownloadUrl(foundClaim.Document);

            return foundClaim.ToClaimAndPreSignedDownloadUrlResponse(preSignedDownloadUrl);
        }
    }
}
