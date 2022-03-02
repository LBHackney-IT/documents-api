using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.Infrastructure;
using Amazon.S3.Model;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;

namespace DocumentsApi.V1.Gateways
{
    public class S3Gateway : IS3Gateway
    {
        private readonly IAmazonS3 _s3;
        private readonly INodeJSService _nodeJSService;
        private readonly AppOptions _options;

        private const string UrlExpirySeconds = "3600";

        public S3Gateway(IAmazonS3 amazonS3, INodeJSService nodeJSService, AppOptions options)
        {
            _s3 = amazonS3;
            _nodeJSService = nodeJSService;
            _options = options;
        }

        public async Task<S3UploadPolicy> GenerateUploadPolicy()
        {
            /* TODO: Node being added to create signed Post Policies
               (see this issue: https://github.com/LBHackney-IT/documents-api/pull/6)
               Can be removed when presigned post policies are available in .NET
             */
            //var policyString = await _nodeServices.InvokeAsync<string>("V1/Node/index.js", _options.DocumentsBucketName, "pre-scan/" + document.Id.ToString(), UrlExpirySeconds).ConfigureAwait(true);
            string javascriptModule =
                @"const { createPresignedPost } = require('@aws-sdk/s3-presigned-post');
                const { S3Client } = require('@aws-sdk/client-s3');

                /* TODO: Node being added to create signed Post Policies
                (see this issue: https://github.com/LBHackney-IT/documents-api/pull/6)
                Can be removed when presigned post policies are available in .NET
                */
                module.exports = (callback, bucketName, key, expiry) => {
                    try {
                        const expiryInSeconds = parseInt(expiry);
                        const client = new S3Client({
                            region: 'eu-west-2',
                            endpoint: process.env.S3_API_ENDPOINT,
                        });
                        const data = createPresignedPost(client, {
                            Bucket: bucketName,
                            Key: key,
                            Expires: expiryInSeconds,
                            Fields: {
                                acl: 'private',
                                key,
                                'X-Amz-Server-Side-Encryption': 'AES256',
                            },
                            Conditions: [
                                { bucket: bucketName },
                                { acl: 'private' },
                                { key },
                                { 'x-amz-server-side-encryption': 'AES256' },
                                ['content-length-range', 1, 500000000], // 500mb
                                ['starts-with', '$Content-Type', ''],
                            ],
                        });

                        return callback(null, JSON.stringify(data));
                    } catch (err) {
                        console.log('Failed generating pre-signed upload url', {
                            error: err,
                        });

                        return callback(err);
                    }
                };
            ";
            var policyString = await _nodeJSService.InvokeFromStringAsync<string>(javascriptModule, args: new[] { _options.DocumentsBucketName, "pre-scan/" + "test-name", UrlExpirySeconds }).ConfigureAwait(true);
            Console.WriteLine($"The S3 link is: {policyString}");
            return JsonConvert.DeserializeObject<S3UploadPolicy>(policyString);
        }

        public async Task<string> GetObjectContentType(string key)
        {
            var meta = await _s3.GetObjectMetadataAsync(_options.DocumentsBucketName, key).ConfigureAwait(true);
            return meta.Headers.ContentType;
        }

        public PutObjectResponse UploadDocument(Guid documentId, Base64DecodedData document)
        {
            var byteArray = Convert.FromBase64String(document.DocumentBase64String);
            using (var stream = new MemoryStream(byteArray))
            {
                var request = new PutObjectRequest
                {
                    BucketName = _options.DocumentsBucketName,
                    Key = "pre-scan/" + documentId,
                    InputStream = stream,
                    ContentType = document.DocumentType
                };
                return _s3.PutObjectAsync(request).Result;
            }
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
