using System;
using System.Threading.Tasks;
using DocumentsApi.V1.Domain;
using DocumentsApi.V1.Factories;
using DocumentsApi.V1.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
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
        public async Task CreatingUploadPolicyReturns404ForNonExistentDocument()
        {
            var uri = new Uri($"api/v1/documents/{Guid.NewGuid()}/upload_policies", UriKind.Relative);
            var response = await Client.PostAsync(uri, null).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task CreatingUploadPolicyReturns400ForAlreadyUploadedDocument()
        {
            var uri = new Uri($"api/v1/documents/{_document.Id}/upload_policies", UriKind.Relative);
            var response = await Client.PostAsync(uri, null).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task CreatingUploadPolicyReturnsPolicyForValidDocument()
        {
            _document.UploadedAt = null;
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/documents/{_document.Id}/upload_policies", UriKind.Relative);
            var response = await Client.PostAsync(uri, null).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var policy = JsonConvert.DeserializeObject<S3UploadPolicy>(jsonString);

            response.StatusCode.Should().Be(201);

            policy.Fields["key"].Should().Be(_document.Id.ToString());
            policy.Fields["acl"].Should().Be("private");
            policy.Fields["X-Amz-Server-Side-Encryption"].Should().Be("AES256");
            policy.Fields["X-Amz-Algorithm"].Should().Be("AWS4-HMAC-SHA256");

            policy.Fields.Should().ContainKeys("bucket", "X-Amz-Credential", "X-Amz-Date", "Policy", "X-Amz-Signature");
        }
    }
}
