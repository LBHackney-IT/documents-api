using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3.Model;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class DocumentsTests : IntegrationTests<Startup>
    {
        private DocumentEntity _document;

        [SetUp]
        public void SetUp()
        {
            var document = TestDataHelper.CreateDocument().ToEntity();
            DatabaseContext.Add(document);
            DatabaseContext.SaveChanges();
            _document = document;
        }

        [Test]
        public async Task CreatingUploadUrlReturns404ForNonExistentDocument()
        {
            var uri = new Uri($"api/v1/documents/{Guid.NewGuid()}/upload_urls", UriKind.Relative);
            var response = await Client.PostAsync(uri, null).ConfigureAwait(true);
            // var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task CreatingUploadUrlReturnsUrlForValidDocument()
        {
            var expectedUrl = "https://upload.s3.com";
            MockS3Client.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(expectedUrl);
            var uri = new Uri($"api/v1/documents/{_document.Id}/upload_urls", UriKind.Relative);
            var response = await Client.PostAsync(uri, null).ConfigureAwait(true);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(201);

            json.Should().Be($"{{\"url\":\"{expectedUrl}\"}}");
        }
    }
}
