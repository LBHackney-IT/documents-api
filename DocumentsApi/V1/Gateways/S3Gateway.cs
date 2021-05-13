using System;
using System.Threading.Tasks;
using Amazon.S3;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Amazon.S3.Model;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.Gateways
{
    public class S3Gateway : IS3Gateway
    {
        private readonly IAmazonS3 _s3;
        private readonly INodeServices _nodeServices;
        private readonly AppOptions _options;

        private const int UrlExpirySeconds = 3600;

        public S3Gateway(IAmazonS3 amazonS3, INodeServices nodeServices, AppOptions options)
        {
            _s3 = amazonS3;
            _nodeServices = nodeServices;
            _options = options;
        }

        public async Task<S3UploadPolicy> GenerateUploadPolicy(Document document)
        {
            /* TODO: Node being added to create signed Post Policies
               (see this issue: https://github.com/LBHackney-IT/documents-api/pull/6)
               Can be removed when presigned post policies are available in .NET
             */
            var policyString = await _nodeServices.InvokeAsync<string>("V1/Node/index.js", _options.DocumentsBucketName, "pre-scan/" + document.Id.ToString(), UrlExpirySeconds).ConfigureAwait(true);
            return JsonConvert.DeserializeObject<S3UploadPolicy>(policyString);
        }

        public async Task<string> GetObjectContentType(string key)
        {
            var meta = await _s3.GetObjectMetadataAsync(_options.DocumentsBucketName, key).ConfigureAwait(true);
            return meta.Headers.ContentType;
        }

        // Suppress CA1055 because GetPreSignedURL returns a string, not an Uri
        [SuppressMessage("ReSharper", "CA1055")]
        [SuppressMessage("ReSharper", "CA2200")]
        public string GeneratePreSignedDownloadUrl(Document document)
        {
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
            {
                BucketName = _options.DocumentsBucketName,
                Key = "clean/" + document.Id.ToString(),
                Expires = DateTime.UtcNow.AddMinutes(10)
            };
            var urlString = "";
            try
            {
                urlString = _s3.GetPreSignedURL(request);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the presigned URL: '{0}' ", e.Message);
                throw e;
            }
            return urlString;
        }
    }
}
