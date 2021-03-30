using System;
using System.Collections.Generic;
using Amazon.Lambda.S3Events;
using Amazon.S3.Util;
using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;

namespace DocumentsApi.Tests.V1
{
    public static class TestDataHelper
    {
        private static Fixture _fixture = new Fixture();

        public static Document CreateDocument()
        {
            return _fixture.Build<Document>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .Create();
        }

        public static Claim CreateClaim()
        {
            var document = CreateDocument();
            return _fixture.Build<Claim>()
                .Without(x => x.Id)
                .Without(x => x.CreatedAt)
                .With(x => x.Document, document)
                .With(x => x.RetentionExpiresAt, DateTime.UtcNow.AddDays(2))
                .Create();
        }

        public static S3Event CreateS3Event(List<Guid> ids)
        {
            var records = ids.ConvertAll(id =>
            {
                var record = _fixture.Create<S3EventNotification.S3EventNotificationRecord>();
                record.S3.Object.Key = id.ToString();

                return record;
            });

            var s3Event = _fixture.Build<S3Event>()
                .With(x => x.Records, records)
                .Create();

            return s3Event;
        }

    }
}
