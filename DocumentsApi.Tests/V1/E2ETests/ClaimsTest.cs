using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Factories;
using FluentAssertions;
using FluentAssertions.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DocumentsApi.Tests.V1.E2ETests
{
    public class ClaimsTest : IntegrationTests<Startup>
    {
        [Test]
        public async Task CanCreateClaimsWithValidParams()
        {
            var uri = new Uri($"api/v1/claims", UriKind.Relative);
            var retentionExpiresAt = DateTime.Now.AddDays(3);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(retentionExpiresAt);
            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": \"evidence-api\"," +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}" +
                "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(201);

            var created = DatabaseContext.Claims.First();
            var document = DatabaseContext.Documents.First();

            var formattedCreatedAt = JsonConvert.SerializeObject(created.CreatedAt.ToDateTimeOffset());
            var formattedDocumentCreatedAt = JsonConvert.SerializeObject(document.CreatedAt.ToDateTimeOffset());
            string expected = "{" +
                              $"\"id\":\"{created.Id}\"," +
                              $"\"createdAt\":{formattedCreatedAt}," +
                              "\"document\":{" +
                                  $"\"id\":\"{document.Id}\"," +
                                  $"\"createdAt\":{formattedDocumentCreatedAt}," +
                                  "\"fileSize\":0," +
                                  "\"fileType\":null," +
                                  "\"uploadedAt\":null" +
                              "}," +
                              "\"serviceAreaCreatedBy\":\"development-team-staging\"," +
                              "\"userCreatedBy\":\"staff@test.hackney.gov.uk\"," +
                              "\"apiCreatedBy\":\"evidence-api\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}" +
                              "}";

            json.Should().Be(expected);
        }

        [Test]
        public async Task ReturnsBadRequestWithInvalidParams()
        {
            var uri = new Uri($"api/v1/claims", UriKind.Relative);
            var retentionExpiresAt = DateTime.Now.AddDays(3);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(retentionExpiresAt);
            string body = "{" +
                          "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                          "\"userCreatedBy\": \"\"," +
                          "\"apiCreatedBy\": \"evidence-api\"," +
                          $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }


        [Test]
        public async Task FindingAValidClaimReturnsDocumentResponse()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims/{claim.Id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<ClaimResponse>(jsonString);

            response.StatusCode.Should().Be(200);

            result.Should().BeEquivalentTo(claim.ToDomain().ToResponse());
            result.Document.Should().BeEquivalentTo(claim.Document.ToDomain().ToResponse());
        }

        [Test]
        public async Task FindingAnInvalidDocumentReturns404()
        {
            var uri = new Uri($"api/v1/claims/{Guid.NewGuid()}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task ReturnsDownloadUrlWhenDocumentIsFound()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            claim.Id = Guid.NewGuid();
            var document = TestDataHelper.CreateDocument().ToEntity();
            DatabaseContext.Add(document);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims/{claim.Id}/download_links", UriKind.Relative);
            string body = "{" +
                          $"\"documentId\": \"{document.Id}\"" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(201);
        }

        [Test]
        public async Task Returns404WhenCannotFindDocument()
        {
            var claimId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var uri = new Uri($"api/v1/claims/{claimId}/download_links", UriKind.Relative);
            string body = "{" +
                          $"\"documentId\": \"{documentId}\"" +
                          "}";
            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

    }
}
