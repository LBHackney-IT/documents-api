using System;
using System.IO;
using System.Text.RegularExpressions;
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

        public PutObjectResponse UploadDocument(Guid documentId, DocumentUploadRequest documentUploadRequest)
        {
            var base64Decoded = DecodeBase64ImageString(documentUploadRequest.Base64Document);
            var byteArray = Convert.FromBase64String(base64Decoded.Imagebase64String);
            using (var stream = new MemoryStream(byteArray))
            {
                var request = new PutObjectRequest
                {
                    BucketName = _options.DocumentsBucketName,
                    Key = "pre-scan/" + documentId,
                    InputStream = stream,
                    ContentType = base64Decoded.ImageType
                };
                return _s3.PutObjectAsync(request).Result;
            }
        }

        private static Base64DecodedData DecodeBase64ImageString(string imageString)
        {
            var base64Part = imageString.Split(",")[1];
            var fileTypePart = Regex.Match(imageString, @"(?<=:).+(?=;)").Value;
            var fileExt = Regex.Match(fileTypePart, @"(?<=\/).+").Value;

            return new Base64DecodedData()
            {
                Imagebase64String = base64Part,
                ImageType = fileTypePart,
                ImageExtension = fileExt
            };
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
