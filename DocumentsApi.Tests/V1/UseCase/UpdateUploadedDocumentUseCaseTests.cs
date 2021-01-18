using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;
using AutoFixture;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Gateways.Interfaces;
using DocumentsApi.V1.UseCase;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.UseCase
{
    [TestFixture]
    public class UpdateUploadedDocumentUseCaseTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IDocumentsGateway> _documentsGateway = new Mock<IDocumentsGateway>();
        private readonly Mock<IS3Gateway> _s3Gateway = new Mock<IS3Gateway>();
        private UpdateUploadedDocumentUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _classUnderTest = new UpdateUploadedDocumentUseCase(_documentsGateway.Object, _s3Gateway.Object);
        }

        [Test]
        public async Task UpdatesDocumentState()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var s3Event = CreateEvent(new List<Guid> {id1, id2});

            var document1 = CreateDocument(id1);
            var document2 = CreateDocument(id2);
            var contentType1 = "image/jpeg";
            var contentType2 = "image/png";

            _s3Gateway.Setup(x => x.GetObjectContentType(id1.ToString())).ReturnsAsync(contentType1);
            _s3Gateway.Setup(x => x.GetObjectContentType(id2.ToString())).ReturnsAsync(contentType2);

            _documentsGateway.Setup(x => x.FindDocument(id1)).Returns(document1);
            _documentsGateway.Setup(x => x.FindDocument(id2)).Returns(document2);

            _documentsGateway.Setup(x => x.CreateDocument(It.Is<Document>(doc =>
                doc.Id == id1 && doc.FileType == contentType1 && doc.FileSize == s3Event.Records[0].S3.Object.Size &&
                doc.UploadedAt == s3Event.Records[0].EventTime)));
            _documentsGateway.Setup(x => x.CreateDocument(It.Is<Document>(doc =>
                doc.Id == id2 && doc.FileType == contentType2 && doc.FileSize == s3Event.Records[1].S3.Object.Size &&
                doc.UploadedAt == s3Event.Records[1].EventTime)));

            await _classUnderTest.ExecuteAsync(s3Event).ConfigureAwait(true);

            _s3Gateway.VerifyAll();
            _documentsGateway.VerifyAll();
        }

        [Test]
        public async Task SkipsIfDocumentIsAlreadyUploaded()
        {
            var id = Guid.NewGuid();
            var s3Event = CreateEvent(new List<Guid> {id});

            var document = _fixture.Create<Document>();
            var contentType1 = "image/jpeg";

            _s3Gateway.Setup(x => x.GetObjectContentType(id.ToString())).ReturnsAsync(contentType1);
            _documentsGateway.Setup(x => x.FindDocument(id)).Returns(document);

            await _classUnderTest.ExecuteAsync(s3Event).ConfigureAwait(true);

            _documentsGateway.Verify(dg => dg.CreateDocument(document), Times.Never);
        }

        [Test]
        public async Task SkipsIfDocumentNonExistent()
        {
            var id = Guid.NewGuid();
            var s3Event = CreateEvent(new List<Guid> {id});

            var document = CreateDocument(id);
            var contentType1 = "image/jpeg";

            _s3Gateway.Setup(x => x.GetObjectContentType(id.ToString())).ReturnsAsync(contentType1);

            await _classUnderTest.ExecuteAsync(s3Event).ConfigureAwait(true);

            _documentsGateway.Verify(dg => dg.CreateDocument(document), Times.Never);
        }

        [Test]
        public async Task SkipsIfErrorOccurs()
        {
            var id = Guid.NewGuid();
            var s3Event = CreateEvent(new List<Guid> {id});

            var document = CreateDocument(id);

            _s3Gateway.Setup(x => x.GetObjectContentType(id.ToString())).Throws(new AmazonS3Exception("oh no"));

            await _classUnderTest.ExecuteAsync(s3Event).ConfigureAwait(true);

            _documentsGateway.Verify(dg => dg.CreateDocument(document), Times.Never);
        }

        private S3Event CreateEvent(List<Guid> ids)
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

        private Document CreateDocument(Guid id)
        {
            var document = _fixture.Build<Document>()
                .With(x => x.Id, id)
                .Without(x => x.FileSize)
                .Without(x => x.FileType)
                .Without(x => x.UploadedAt)
                .Create();

            return document;
        }
    }
}
