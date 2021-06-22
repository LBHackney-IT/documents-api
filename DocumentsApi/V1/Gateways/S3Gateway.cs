using System;
using System.Threading.Tasks;
using Amazon.S3;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using Amazon.S3.Model;
using DocumentsApi.V1.Boundary.Request;

namespace DocumentsApi.V1.Gateways
{
    public class S3Gateway : IS3Gateway
    {
        private readonly IAmazonS3 _s3;
        private readonly AppOptions _options;

        public S3Gateway(IAmazonS3 amazonS3, AppOptions options)
        {
            _s3 = amazonS3;
            _options = options;
        }

        public async Task<string> GetObjectContentType(string key)
        {
            var meta = await _s3.GetObjectMetadataAsync(_options.DocumentsBucketName, key).ConfigureAwait(true);
            return meta.Headers.ContentType;
        }

        public PutObjectResponse UploadDocument(DocumentUploadRequest documentUploadRequest)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = _options.DocumentsBucketName,
                Key = "pre-scan/" + documentUploadRequest.Id,
                InputStream = documentUploadRequest.Document.OpenReadStream(),
                ContentType = documentUploadRequest.Document.ContentType
            };
            return _s3.PutObjectAsync(request).Result;
        }

        public GetObjectResponse GetObject(Document document)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = _options.DocumentsBucketName,
                    Key = "clean/" + document.Id
                };
                return _s3.GetObjectAsync(request).Result;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the S3 object: '{0}' ", e.Message);
                throw;
            }
        }
    }
}
