using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentsApi.V1.UseCase
{
    public class UpdateUploadedDocumentUseCase : IUpdateUploadedDocumentUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;
        private readonly IS3Gateway _s3Gateway;
        private readonly ILogger<UpdateUploadedDocumentUseCase> _logger;

        public UpdateUploadedDocumentUseCase(
            IDocumentsGateway documentsGateway,
            IS3Gateway s3Gateway,
            ILogger<UpdateUploadedDocumentUseCase> logger)
        {
            _documentsGateway = documentsGateway;
            _s3Gateway = s3Gateway;
            _logger = logger;
        }

        public async Task ExecuteAsync(S3Event s3Event)
        {
            var tasks = s3Event.Records.Select(UpdateDocument);

            await Task.WhenAll(tasks).ConfigureAwait(true);
        }

        // Catch everything because this runs asynchronously
        [SuppressMessage("ReSharper", "CA1031")]
        private async Task UpdateDocument(S3EventNotification.S3EventNotificationRecord record)
        {
            var documentKey = record.S3.Object.Key;
            _logger.LogInformation("Processing key {documentKey}", documentKey);

            var splitArray = documentKey.Split('/');
            var documentId = splitArray.Length > 1 ? splitArray[1] : splitArray[0];
            _logger.LogInformation("Processing document with ID {documentId}", documentId);

            try
            {
                var size = record.S3.Object.Size;
                var uploadedAt = record.EventTime;
                var document = _documentsGateway.FindDocument(new Guid(documentId));

                if (document == null)
                {
                    _logger.LogInformation("Could not find document with ID {documentId}", documentId);
                    return;
                }

                if (document.Uploaded)
                {
                    _logger.LogInformation("Document with ID {documentId} has already been uploaded", documentId);
                    return;
                }

                var type = await _s3Gateway.GetObjectContentType(documentKey).ConfigureAwait(true);

                document.UploadedAt = uploadedAt;
                document.FileSize = size;
                document.FileType = type;

                _documentsGateway.SaveDocument(document);
                _logger.LogInformation("Completed processing document with ID {documentId}", documentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process document with ID {documentId}", documentId);
                _logger.LogError(ex.Message);
            }
        }
    }
}
