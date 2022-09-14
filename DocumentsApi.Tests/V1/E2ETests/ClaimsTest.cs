using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Factories;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Moq;
using Amazon.S3.Model;

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
                $"\"validUntil\": {formattedValidUntil}," +
                "\"targetId\": \"eaed0ee5-d88c-4cf1-9df9-268a24ea0450\"," +
                "\"documentName\": \"Some name\"" +
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
                                  "\"name\":\"Some name\"," +
                                  "\"fileSize\":0," +
                                  "\"fileType\":null," +
                                  "\"uploadedAt\":null" +
                              "}," +
                              "\"serviceAreaCreatedBy\":\"development-team-staging\"," +
                              "\"userCreatedBy\":\"staff@test.hackney.gov.uk\"," +
                              "\"apiCreatedBy\":\"evidence-api\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}," +
                              $"\"validUntil\":{formattedValidUntil}," +
                              "\"targetId\":\"eaed0ee5-d88c-4cf1-9df9-268a24ea0450\"" +
                              "}";

            json.Should().Be(expected);
        }

        [Test]
        public async Task CanCreateClaimsWithoutTargetId()
        {
            var uri = new Uri($"api/v1/claims", UriKind.Relative);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(3));
            var formattedValidUntil = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(4));
            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": \"evidence-api\"," +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}," +
                $"\"validUntil\": {formattedValidUntil}," +
                "\"documentName\": \"Some name\"" +
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
                                  "\"name\":\"Some name\"," +
                                  "\"fileSize\":0," +
                                  "\"fileType\":null," +
                                  "\"uploadedAt\":null" +
                              "}," +
                              "\"serviceAreaCreatedBy\":\"development-team-staging\"," +
                              "\"userCreatedBy\":\"staff@test.hackney.gov.uk\"," +
                              "\"apiCreatedBy\":\"evidence-api\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}," +
                              $"\"validUntil\":{formattedValidUntil}," +
                              "\"targetId\":null" +
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
        public async Task Returns200WhenItCanGetClaimAndPreSignedDownloadUrl()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.Add(claim.Document);
            DatabaseContext.SaveChanges();

            var expectedPreSignedDownloadUrl = new String("www.awsDownloadUrl.com");
            MockS3Client.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(expectedPreSignedDownloadUrl);

            var uri = new Uri($"api/v1/claims/claim_and_download_url/{claim.Id}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<ClaimAndPreSignedDownloadUrlResponse>(jsonString);

            response.StatusCode.Should().Be(200);
            result.PreSignedDownloadUrl.Should().Be(expectedPreSignedDownloadUrl);
        }

        [Test]
        public async Task Returns400WhenGetClaimAndPreSignedDownloadUrlRequestIsNotValid()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.Add(claim.Document);
            DatabaseContext.SaveChanges();
            var fakeClaimId = Guid.NewGuid();

            var uri = new Uri($"api/v1/claims/claim_and_download_url/${fakeClaimId}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<ClaimAndPreSignedDownloadUrlResponse>(jsonString);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task ClaimAndS3UploadRequestReturnsUploadPolicy()
        {
            var uri = new Uri($"api/v1/claims/claim_and_upload_policy", UriKind.Relative);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(3));
            var formattedValidUntil = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(4));

            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": \"some-api\"," +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}," +
                $"\"validUntil\": {formattedValidUntil}" +
                "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString);
            var readResponse = await response.Content.ReadAsStringAsync();
            var responseAsObject = JsonConvert.DeserializeObject<ClaimAndS3UploadPolicyResponse>(readResponse);

            response.StatusCode.Should().Be(201);
            responseAsObject.S3UploadPolicy.Url.Should().NotBeNull();
        }

        [Test]
        public async Task ClaimAndS3UploadRequestReturnsBadRequestWithInvalidParams()
        {
            var uri = new Uri($"api/v1/claims/claim_and_upload_policy", UriKind.Relative);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(3));
            var formattedValidUntil = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(4));

            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": " +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}," +
                $"\"validUntil\": {formattedValidUntil}" +
                "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }


        [Test]
        public async Task ReturnsDownloadUrlWhenClaimIsFound()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            var claimId = Guid.NewGuid();
            claim.Id = claimId;

            DatabaseContext.Add(claim);
            DatabaseContext.SaveChanges();

            //S3Proxy for mocks is not handling .GetPresignedURL correctly, so it is mocked manually here
            var expectedUrlFromS3 = "www.s3urlstring";
            MockS3Client.Setup(x => x.GetPreSignedURL(It.IsAny<GetPreSignedUrlRequest>())).Returns(expectedUrlFromS3);

            var uri = new Uri($"api/v1/claims/{claimId}/download_links", UriKind.Relative);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Returns404WhenCannotFindClaimForDownloadUrl()
        {
            var nonExistentClaimId = Guid.NewGuid();

            var uri = new Uri($"api/v1/claims/{nonExistentClaimId}/download_links", UriKind.Relative);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task Returns200WhenItCanGetClaimsByTargetId()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            claim.Document.FileSize = 0;
            claim.Document.FileType = null;
            claim.Document.UploadedAt = null;
            DatabaseContext.Add(claim);
            DatabaseContext.Add(claim.Document);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims?targetId={claim.TargetId}", UriKind.Relative);
            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<Dictionary<string, List<ClaimResponse>>>(jsonString);
            
            var formattedCreatedAt = JsonConvert.SerializeObject(claim.CreatedAt);
            var formattedDocumentCreatedAt = JsonConvert.SerializeObject(claim.Document.CreatedAt);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(claim.RetentionExpiresAt);
            var formattedValidUntil = JsonConvert.SerializeObject(claim.ValidUntil);
        
            string expected = "{" +
                            "\"claims\":[{" +
                              $"\"id\":\"{claim.Id}\"," +
                              $"\"createdAt\":{formattedCreatedAt}," +
                              "\"document\":{" +
                                  $"\"id\":\"{claim.Document.Id}\"," +
                                  $"\"createdAt\":{formattedDocumentCreatedAt}," +
                                  $"\"name\":\"{claim.Document.Name}\"," +
                                  $"\"fileSize\":0," +
                                  $"\"fileType\":null," +
                                  $"\"uploadedAt\":null" +
                              "}," +
                              $"\"serviceAreaCreatedBy\":\"{claim.ServiceAreaCreatedBy}\"," +
                              $"\"userCreatedBy\":\"{claim.UserCreatedBy}\"," +
                              $"\"apiCreatedBy\":\"{claim.ApiCreatedBy}\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}," +
                              $"\"validUntil\":{formattedValidUntil}," +
                              $"\"targetId\":\"{claim.TargetId}\"" +
                              "}" +
                            "]}";

            response.StatusCode.Should().Be(200);
            jsonString.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Returns404WhenCannotFindClaimsForTargetId()
        {
            var nonExistentTargetId = Guid.NewGuid();

            var uri = new Uri($"api/v1/claims?targetId={nonExistentTargetId}", UriKind.Relative);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task Returns400WhenTargetIdIsNotGuid()
        {
            var invalidTargetId = "abc";

            var uri = new Uri($"api/v1/claims?targetId={invalidTargetId}", UriKind.Relative);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }
    }
}
