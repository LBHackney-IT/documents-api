using System;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Amazon.S3;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

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

        [SuppressMessage("ReSharper", "CA2200")]
        public Task<Stream> Execute(Guid documentId)
        {
            var document = _documentsGateway.FindDocument(documentId);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID: {documentId}");
            }

            Task<Stream> result;

            try
            {
                //result = _s3Gateway.GetObject(document);
                result = _s3Gateway.GetObjectFromLocal();
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the S3 object: '{0}' ", e.Message);
                throw e;
            }

            return result;
        }
    }
}
