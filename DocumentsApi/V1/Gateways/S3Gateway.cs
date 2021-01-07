using System;
using Amazon.S3;
using Amazon.S3.Model;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;

namespace DocumentsApi.V1.Gateways
{
    public class S3Gateway : IS3Gateway
    {
        private IAmazonS3 _s3;

        public const double UrlExpiryMinutes = 60;

        public S3Gateway(IAmazonS3 amazonS3)
        {
            _s3 = amazonS3;
        }

        public Uri GenerateUploadUrl(Document document)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = AppOptions.DocumentsBucketName,
                Expires = DateTime.UtcNow.AddMinutes(UrlExpiryMinutes),
                Key = document.Id.ToString(),
                Verb = HttpVerb.PUT
            };

            var url = _s3.GetPreSignedURL(request);
            return new Uri(url);
        }
    }
}
