using System;
using System.IO;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Amazon.S3;
using System.Threading.Tasks;
using DocumentsApi.V1.Domain;

namespace DocumentsApi.V1.UseCase
{
    public class DownloadDocumentUseCase : IDownloadDocumentUseCase
    {
        private IS3Gateway _s3Gateway;
        private IDocumentsGateway _documentsGateway;

        public DownloadDocumentUseCase(IS3Gateway s3Gateway, IDocumentsGateway documentsGateway)
        {
            _s3Gateway = s3Gateway;
            _documentsGateway = documentsGateway;
        }

        public Tuple<Document, Task<Stream>> Execute(Guid documentId)
        {
            var document = _documentsGateway.FindDocument(documentId);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID: {documentId}");
            }

            try
            {
                var result = _s3Gateway.GetObject(document);
                return new Tuple<Document, Task<Stream>>(document, result);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the S3 object: '{0}' ", e.Message);
                throw;
            }
        }
    }
}
