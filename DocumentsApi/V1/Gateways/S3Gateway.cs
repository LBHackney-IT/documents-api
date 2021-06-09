using System;
using System.Threading.Tasks;
using Amazon.S3;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using System.IO;
using Amazon.S3.Model;

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

        public async Task<Stream> GetObject(Document document)
        {
            try
            {
                var key = "clean/" + document.Id;
                using (var responseStream = await _s3.GetObjectStreamAsync(_options.DocumentsBucketName, key, null)
                    .ConfigureAwait(true))
                {
                    var stream = new MemoryStream();
                    await responseStream.CopyToAsync(stream).ConfigureAwait(true);
                    stream.Position = 0;
                    return stream;
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error when retrieving the S3 object: '{0}' ", e.Message);
                throw;
            }
        }

        // remove once we finish testing
        public async Task<Stream> GetObjectFromLocal()
        {
            try
            {
                string path = "/Users/bogdan/Desktop/demo1.jpeg";
                var file = File.Open(path, System.IO.FileMode.Open);
                using (var getObjectResponse = file)
                {
                    using (var responseStream = file)
                    {
                        var stream = new MemoryStream();
                        await responseStream.CopyToAsync(stream).ConfigureAwait(true);
                        stream.Position = 0;
                        return stream;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Error when retrieving the local file: '{0}' ", e.Message);
                throw;
            }
        }
    }
}
