using System;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Amazon.S3;
using System.Diagnostics.CodeAnalysis;

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
        public string Execute(string documentId)
        {
            var documentGuid = new Guid(documentId);
            var document = _documentsGateway.FindDocument(documentGuid);

            if (document == null)
            {
                throw new NotFoundException($"Cannot find document with ID: {documentGuid}");
            }

            var result = "";

            try
            {
                result = _s3Gateway.GeneratePreSignedDownloadUrl(document);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the presigned URL: '{0}' ", e.Message);
                throw e;
            }

            return result;
        }
    }
}