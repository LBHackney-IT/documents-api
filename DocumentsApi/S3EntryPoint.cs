using System;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3.Util;

[assembly: LambdaSerializer(typeof(JsonSerializer))]
namespace DocumentsApi
{
    public class S3EntryPoint
    {
        private S3EventNotification.S3Entity _s3;
        public void DocumentCreated(S3Event s3Event)
        {
            foreach (var record in s3Event.Records)
            {
                _s3 = record.S3;
                Console.WriteLine($"[{record.EventSource} - {record.EventTime}] Bucket = {_s3.Bucket.Name}, Key = {_s3.Object.Key}");
            }
        }
    }
}
