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

        public HttpStatusCode Execute(DocumentUploadRequest request)
        {
            var document = _documentsGateway.FindDocument(request.Id);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID {request.Id}");
            }

            if (document.Uploaded)
            {
                throw new BadRequestException($"Document with ID {request.Id} has already been uploaded.");
            }

            try
            {
                return _s3Gateway.UploadDocument(request).HttpStatusCode;
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error when uploading DocumentId: '{0}'. Error: '{1}'", request.Id, e.Message);
                throw;
            }
        }
    }
}
