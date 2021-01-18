using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;

namespace DocumentsApi.V1.UseCase
{
    public class UpdateUploadedDocumentUseCase : IUpdateUploadedDocumentUseCase
    {
        private readonly IDocumentsGateway _documentsGateway;
        private readonly IS3Gateway _s3Gateway;

        public UpdateUploadedDocumentUseCase(IDocumentsGateway documentsGateway, IS3Gateway s3Gateway)
        {
            _documentsGateway = documentsGateway;
            _s3Gateway = s3Gateway;
        }

        public async Task ExecuteAsync(S3Event s3Event)
        {
            foreach (var record in s3Event.Records)
            {
                await UpdateDocument(record).ConfigureAwait(true);
            }
        }

        // Catch everything because this runs asynchronously
        [SuppressMessage("ReSharper", "CA1031")]
        private async Task UpdateDocument(S3EventNotification.S3EventNotificationRecord record)
        {
            var id = record.S3.Object.Key;
            Console.WriteLine($"Processing document with ID {id}");

            try
            {
                var size = record.S3.Object.Size;
                var uploadedAt = record.EventTime;
                var type = await _s3Gateway.GetObjectContentType(id).ConfigureAwait(true);

                var document = _documentsGateway.FindDocument(new Guid(id));

                if (document == null)
                {
                    Console.WriteLine($"Could not find document with ID {id}");
                    return;
                }

                if (document.Uploaded)
                {
                    Console.WriteLine($"Document with ID {id} has already been uploaded");
                    return;
                }

                document.UploadedAt = uploadedAt;
                document.FileSize = size;
                document.FileType = type;

                _documentsGateway.CreateDocument(document);
                Console.WriteLine($"Completed processing document with ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process document with ID {id} | {ex.GetType()} {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
