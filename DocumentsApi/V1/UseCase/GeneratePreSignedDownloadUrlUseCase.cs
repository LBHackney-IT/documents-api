using System;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Amazon.S3;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.UseCase
{
    public class GeneratePreSignedDownloadUrlUseCase : IGeneratePreSignedDownloadUrlUseCase
    {
        private IS3Gateway _s3Gateway;
        private IDocumentsGateway _documentsGateway;

        public GeneratePreSignedDownloadUrlUseCase(IS3Gateway s3Gateway, IDocumentsGateway documentsGateway)
        {
            _s3Gateway = s3Gateway;
            _documentsGateway = documentsGateway;
        }
        [SuppressMessage("ReSharper", "CA2200")]
        public string Execute(Guid claimId)
        {
            var claim = _documentsGateway.FindClaim(claimId);

            if (claim == null)
            {
                throw new NotFoundException($"Cannot find a claim with ID: {claimId}");
            }

            try
            {
                var result = _s3Gateway.GeneratePreSignedDownloadUrl(claim.Document);
                return result;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error when retrieving the presigned URL: {e.Message}");
                throw e;
            }
        }
    }
}
