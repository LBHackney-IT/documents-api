using System;
using System.Net;
using Amazon.S3;
using DocumentsApi.V1.Boundary.Request;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class UploadDocumentUseCase : IUploadDocumentUseCase
    {
        private IS3Gateway _s3Gateway;
        private IDocumentsGateway _documentsGateway;
        private readonly ILogger<UploadDocumentUseCase> _logger;

        public UploadDocumentUseCase(IS3Gateway s3Gateway, IDocumentsGateway documentsGateway, ILogger<UploadDocumentUseCase> logger)
        {
            _s3Gateway = s3Gateway;
            _documentsGateway = documentsGateway;
            _logger = logger;
        }

        public void Execute(Guid documentId, DocumentUploadRequest request)
        {
            var document = _documentsGateway.FindDocument(documentId);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID {documentId}");
            }

            if (document.Uploaded)
            {
                throw new BadRequestException($"Document with ID {documentId} has already been uploaded.");
            }

            try
            {
                var result = _s3Gateway.UploadDocument(documentId, request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new DocumentUploadException(result.HttpStatusCode.ToString());
                }
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error when uploading DocumentId: '{0}'. Error: '{1}'", documentId, e.Message);
                throw;
            }
        }
    }
}
