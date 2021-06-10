using System;
using System.IO;
using DocumentsApi.V1.Boundary.Response.Exceptions;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase.Interfaces;
using Amazon.S3;
using Amazon.S3.Model;

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
                var documentAsBase64 = EncodeStreamToBase64(result);
                return documentAsBase64;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the S3 object: '{0}' ", e.Message);
                throw;
            }
        }

        public static string EncodeStreamToBase64(GetObjectResponse s3Response)
        {
            if (s3Response.ResponseStream != null)
            {
                using (Stream responseStream = s3Response.ResponseStream)
                {
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        responseStream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    return $"data:{s3Response.Headers.ContentType};base64," + Convert.ToBase64String(bytes);
                }
            }

            return string.Empty;
        }
    }
}
