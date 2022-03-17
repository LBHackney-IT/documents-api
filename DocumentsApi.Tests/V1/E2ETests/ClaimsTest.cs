using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Factories;
using FluentAssertions;
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
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(3));
            var formattedValidUntil = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(4));
            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": \"evidence-api\"," +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}," +
                $"\"validUntil\": {formattedValidUntil}" +
                "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(201);

            var created = DatabaseContext.Claims.First();
            var document = DatabaseContext.Documents.First();

            var formattedCreatedAt = JsonConvert.SerializeObject(created.CreatedAt);
            var formattedDocumentCreatedAt = JsonConvert.SerializeObject(document.CreatedAt);
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
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}," +
                              $"\"validUntil\":{formattedValidUntil}" +
                              "}";

            json.Should().Be(expected);
        }

        [Test]
        public async Task ReturnsBadRequestWithInvalidParams()
        {
            var uri = new Uri($"api/v1/claims", UriKind.Relative);
            var retentionExpiresAt = DateTime.UtcNow.AddDays(3);
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
        public async Task CanUpdateClaimWithValidParams()
        {
            // Arrange
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.SaveChanges();

            var validUntil = DateTime.UtcNow.AddDays(4);
            var formattedValidUntil = JsonConvert.SerializeObject(validUntil);
            string body = "{" +
                          $"\"validUntil\": {formattedValidUntil}" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/claims/{claim.Id}", UriKind.Relative);

            // Act
            var response = await Client.PatchAsync(uri, jsonString).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(200);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<ClaimResponse>(json);

            result.Should().BeEquivalentTo(claim.ToDomain().ToResponse(),
                opts => opts.Excluding(x => x.ValidUntil));
            result.ValidUntil.Should().Be(validUntil);
            result.Document.Should().BeEquivalentTo(claim.Document.ToDomain().ToResponse());
            // Check we have persisted the updated claim with a different ValidUntil date
            DatabaseContext.Claims.Find(claim.Id).ValidUntil.Should().Be(validUntil);
        }

        [Test]
        public async Task Returns404WhenCannotFindClaim()
        {
            // Arrange - do not persist the claim
            var claim = TestDataHelper.CreateClaim().ToEntity();

            var validUntil = DateTime.UtcNow.AddDays(4);
            var formattedValidUntil = JsonConvert.SerializeObject(validUntil);
            string body = "{" +
                          $"\"validUntil\": {formattedValidUntil}" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var uri = new Uri($"api/v1/claims/{claim.Id}", UriKind.Relative);

            // Act
            var response = await Client.PatchAsync(uri, jsonString).ConfigureAwait(true);

            // Assert
            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task Returns200WhenItCanGetClaimAndDocument()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.Add(claim.Document);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims/claim_and_document/{claim.Id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<ClaimAndDocumentResponse>(jsonString);

            response.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Returns400WhenClaimAndDocumentRequestIsNotValid()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.Add(claim.Document);
            DatabaseContext.SaveChanges();
            var fakeClaimId = Guid.NewGuid();

            var uri = new Uri($"api/v1/claims/claim_and_document/${fakeClaimId}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<ClaimAndDocumentResponse>(jsonString);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task ClaimAndDocumentReturnsBadRequestWithInvalidParams()
        {
            var uri = new Uri($"api/v1/claims/claim_and_document", UriKind.Relative);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(3));
            var formattedValidUntil = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(4));
            string base64Document = "data:@file/plain;base64,VGhpcyBpcyBhIHRlc3QgZmlsZQ==";
            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": " +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}," +
                $"\"validUntil\": {formattedValidUntil}," +
                $"\"base64Document\": \"{base64Document}\"" +
                "}";


            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
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
            var nonExistentClaimId = Guid.NewGuid();
            var nonExistentDocumentId = Guid.NewGuid();

            var uri = new Uri($"api/v1/claims/{nonExistentClaimId}/download_links", UriKind.Relative);

            string body = "{" +
                          $"\"documentId\": \"{nonExistentDocumentId}\"" +
                          "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }
    }
}
