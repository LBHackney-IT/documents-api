using System;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Amazon.S3;
using DocumentsApi.V1.Factories;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class DownloadDocumentUseCase : IDownloadDocumentUseCase
    {
        private IS3Gateway _s3Gateway;
        private IDocumentsGateway _documentsGateway;
        private IDocumentFormatFactory _documentFormatFactory;
        private readonly ILogger<DownloadDocumentUseCase> _logger;

        public DownloadDocumentUseCase(
            IS3Gateway s3Gateway,
            IDocumentsGateway documentsGateway,
            IDocumentFormatFactory documentFormatFactory,
            ILogger<DownloadDocumentUseCase> logger)
        {
            _s3Gateway = s3Gateway;
            _documentsGateway = documentsGateway;
            _documentFormatFactory = documentFormatFactory;
            _logger = logger;
        }

        public string Execute(Guid documentId)
        {
            var document = _documentsGateway.FindDocument(documentId);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID: {documentId}");
            }

            try
            {
                var result = _s3Gateway.GetObject(document);
                var documentAsBase64 = _documentFormatFactory.EncodeStreamToBase64(result);
                return documentAsBase64;
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error when retrieving the S3 object: '{0}' ", e.Message);
                throw;
            }
        }
    }
}
