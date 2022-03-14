using System;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        public async Task UploadDocumentReturns404ForNonExistentDocument()
        {
            string body = "{" +
                          "\"base64Document\": \"abcd\"" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/documents/{Guid.NewGuid().ToString()}", UriKind.Relative);
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task UploadDocumentReturns400ForAlreadyUploadedDocument()
        {
            string body = "{" +
                          "\"base64Document\": \"abcd\"" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/documents/{_document.Id}", UriKind.Relative);
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task ReturnsDocumentStringWhenDocumentIsFound()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            claim.Id = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument().ToEntity();
            DatabaseContext.Add(document);
            DatabaseContext.SaveChanges();
            var expected = new GetObjectResponse();
            MockS3Client.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(expected);

            var uri = new Uri($"api/v1/documents/{document.Id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Returns404WhenCannotFindDocument()
        {
            var documentId = Guid.NewGuid();
            var uri = new Uri($"api/v1/documents/{documentId}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task CreatingUploadPolicyReturns404ForNonExistentDocument()
        {
            var uri = new Uri($"api/v1/documents/{Guid.NewGuid()}/upload_policies", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task CreatingUploadPolicyReturns400ForAlreadyUploadedDocument()
        {
            var uri = new Uri($"api/v1/documents/{_document.Id}/upload_policies", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }
    }
}
