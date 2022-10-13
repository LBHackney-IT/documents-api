using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DocumentsApi.V1.Boundary.Response;
using DocumentsApi.V1.Factories;
using DocumentsApi.Tests.V1.E2ETests.Constants;
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
                "\"documentName\": \"Some name\"," +
                "\"documentDescription\": \"Some description\"" +
                "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString);
            var json = await response.Content.ReadAsStringAsync();

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
                                  "\"description\":\"Some description\"," +
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
            var response = await Client.PostAsync(uri, jsonString);
            var json = await response.Content.ReadAsStringAsync();

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
                                  "\"description\":null," +
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
            var response = await Client.PostAsync(uri, jsonString);

            response.StatusCode.Should().Be(400);
        }


        [Test]
        public async Task FindingAValidClaimReturnsDocumentResponse()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            DatabaseContext.Add(claim);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims/{claim.Id}", UriKind.Relative);
            var response = await Client.GetAsync(uri);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClaimResponse>(jsonString);

            response.StatusCode.Should().Be(200);

            result.Should().BeEquivalentTo(claim.ToDomain().ToResponse());
            result.Document.Should().BeEquivalentTo(claim.Document.ToDomain().ToResponse());
        }

        [Test]
        public async Task FindingAnInvalidDocumentReturns404()
        {
            var uri = new Uri($"api/v1/claims/{Guid.NewGuid()}", UriKind.Relative);
            var response = await Client.GetAsync(uri);

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
            var response = await Client.PatchAsync(uri, jsonString);

            // Assert
            response.StatusCode.Should().Be(200);

            var json = await response.Content.ReadAsStringAsync();
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
            var response = await Client.PatchAsync(uri, jsonString);

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
            var response = await Client.GetAsync(uri);
            var jsonString = await response.Content.ReadAsStringAsync();
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
            var response = await Client.GetAsync(uri);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task ClaimAndS3UploadRequestReturnsUploadPolicyAndClaim()
        {
            var uri = new Uri($"api/v1/claims/claim_and_upload_policy", UriKind.Relative);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(3));
            var formattedValidUntil = JsonConvert.SerializeObject(DateTime.UtcNow.AddDays(4));

            string body = "{" +
                "\"serviceAreaCreatedBy\": \"development-team-staging\"," +
                "\"userCreatedBy\": \"staff@test.hackney.gov.uk\"," +
                "\"apiCreatedBy\": \"some-api\"," +
                $"\"retentionExpiresAt\": {formattedRetentionExpiresAt}," +
                $"\"validUntil\": {formattedValidUntil}," +
                "\"documentName\": \"some-name\"," +
                "\"documentDescription\": \"some-description\"" +
                "}";

            var jsonString = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(uri, jsonString);
            var readResponse = await response.Content.ReadAsStringAsync();
            var responseAsObject = JsonConvert.DeserializeObject<ClaimAndS3UploadPolicyResponse>(readResponse);

            response.StatusCode.Should().Be(201);
            responseAsObject.ClaimId.Should().NotBeEmpty();
            responseAsObject.Document.Name.Should().Be("some-name");
            responseAsObject.Document.Description.Should().Be("some-description");
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
            var response = await Client.PostAsync(uri, jsonString);

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

            var response = await Client.GetAsync(uri);

            response.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Returns404WhenCannotFindClaimForDownloadUrl()
        {
            var nonExistentClaimId = Guid.NewGuid();

            var uri = new Uri($"api/v1/claims/{nonExistentClaimId}/download_links", UriKind.Relative);

            var response = await Client.GetAsync(uri);

            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task Returns200WhenItCanGetClaimsByTargetId()
        {
            var claim = TestDataHelper.CreateClaim().ToEntity();
            claim.Id = new Guid("45a04de8-9a52-41d2-a09d-d3d6f5678c89");
            claim.Document.FileSize = 0;
            claim.Document.FileType = null;
            claim.Document.UploadedAt = null;
            DatabaseContext.Add(claim);
            DatabaseContext.Add(claim.Document);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims?targetId={claim.TargetId}", UriKind.Relative);
            Client.DefaultRequestHeaders.Add("Authorization", TestToken.Value);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<PaginatedClaimResponse>(jsonString);

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
                                  $"\"description\":\"{claim.Document.Description}\"," +
                                  "\"fileSize\":0," +
                                  "\"fileType\":null," +
                                  "\"uploadedAt\":null" +
                              "}," +
                              $"\"serviceAreaCreatedBy\":\"{claim.ServiceAreaCreatedBy}\"," +
                              $"\"userCreatedBy\":\"{claim.UserCreatedBy}\"," +
                              $"\"apiCreatedBy\":\"{claim.ApiCreatedBy}\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}," +
                              $"\"validUntil\":{formattedValidUntil}," +
                              $"\"targetId\":\"{claim.TargetId}\"" +
                              "}" +
                            "]," +
                            "\"paging\":{" +
                                "\"cursors\":{" +
                                    "\"before\":\"ewogICJpZCI6ICI0NWEwNGRlOC05YTUyLTQxZDItYTA5ZC1kM2Q2ZjU2NzhjODkiCn0\"," +
                                    "\"after\":\"ewogICJpZCI6ICI0NWEwNGRlOC05YTUyLTQxZDItYTA5ZC1kM2Q2ZjU2NzhjODkiCn0\"" +
                                "}," +
                                "\"hasPreviousPage\":false," +
                                "\"hasNextPage\":false" +
                            "}" +
                        "}";

            response.StatusCode.Should().Be(200);
            jsonString.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Returns200WhenItCanGetClaimsByTargetIdNextPage()
        {
            var targetId = new Guid("6e54aaad-2c5c-4e26-a84c-84563052368d");
            var claim1 = TestDataHelper.CreateClaim().ToEntity();
            claim1.Id = new Guid("45a04de8-9a52-41d2-a09d-d3d6f5678c89");
            claim1.TargetId = targetId;
            claim1.CreatedAt = new DateTime(2022, 9, 5).ToUniversalTime();
            claim1.Document.FileSize = 0;
            claim1.Document.FileType = null;
            claim1.Document.UploadedAt = null;
            DatabaseContext.Add(claim1);
            DatabaseContext.Add(claim1.Document);

            var claim2 = TestDataHelper.CreateClaim().ToEntity();
            claim2.Id = new Guid("ae84084d-0c20-480f-a84e-e131a9a820bb");
            claim2.TargetId = targetId;
            claim2.CreatedAt = new DateTime(2021, 5, 23).ToUniversalTime();
            claim2.Document.FileSize = 0;
            claim2.Document.FileType = null;
            claim2.Document.UploadedAt = null;
            DatabaseContext.Add(claim2);
            DatabaseContext.Add(claim2.Document);

            var claim3 = TestDataHelper.CreateClaim().ToEntity();
            claim3.Id = new Guid("dffbe7a7-f27c-4ec4-bea3-40b57899c8dd");
            claim3.TargetId = targetId;
            claim3.CreatedAt = new DateTime(2020, 4, 18).ToUniversalTime();
            claim3.Document.FileSize = 0;
            claim3.Document.FileType = null;
            claim3.Document.UploadedAt = null;
            DatabaseContext.Add(claim3);
            DatabaseContext.Add(claim3.Document);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims?targetId={targetId}&limit=1&after=ew0KICAgICJpZCI6IjQ1YTA0ZGU4LTlhNTItNDFkMi1hMDlkLWQzZDZmNTY3OGM4OSINCn0", UriKind.Relative);
            Client.DefaultRequestHeaders.Add("Authorization", TestToken.Value);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<PaginatedClaimResponse>(jsonString);

            var formattedCreatedAt = JsonConvert.SerializeObject(claim2.CreatedAt);
            var formattedDocumentCreatedAt = JsonConvert.SerializeObject(claim2.Document.CreatedAt);
            var formattedRetentionExpiresAt = JsonConvert.SerializeObject(claim2.RetentionExpiresAt);
            var formattedValidUntil = JsonConvert.SerializeObject(claim2.ValidUntil);

            string expected = "{" +
                            "\"claims\":[{" +
                              $"\"id\":\"{claim2.Id}\"," +
                              $"\"createdAt\":{formattedCreatedAt}," +
                              "\"document\":{" +
                                  $"\"id\":\"{claim2.Document.Id}\"," +
                                  $"\"createdAt\":{formattedDocumentCreatedAt}," +
                                  $"\"name\":\"{claim2.Document.Name}\"," +
                                  $"\"fileSize\":0," +
                                  $"\"fileType\":null," +
                                  $"\"uploadedAt\":null" +
                              "}," +
                              $"\"serviceAreaCreatedBy\":\"{claim2.ServiceAreaCreatedBy}\"," +
                              $"\"userCreatedBy\":\"{claim2.UserCreatedBy}\"," +
                              $"\"apiCreatedBy\":\"{claim2.ApiCreatedBy}\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt}," +
                              $"\"validUntil\":{formattedValidUntil}," +
                              $"\"targetId\":\"{claim2.TargetId}\"" +
                              "}" +
                            "]," +
                            "\"paging\":{" +
                                "\"cursors\":{" +
                                    "\"before\":\"ewogICJpZCI6ICJhZTg0MDg0ZC0wYzIwLTQ4MGYtYTg0ZS1lMTMxYTlhODIwYmIiCn0\"," +
                                    "\"after\":\"ewogICJpZCI6ICJhZTg0MDg0ZC0wYzIwLTQ4MGYtYTg0ZS1lMTMxYTlhODIwYmIiCn0\"" +
                                "}," +
                                "\"hasPreviousPage\":true," +
                                "\"hasNextPage\":true" +
                            "}" +
                        "}";

            response.StatusCode.Should().Be(200);
            jsonString.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Returns200WhenItCanGetClaimsByTargetIdPreviousPage()
        {
            var targetId = new Guid("6e54aaad-2c5c-4e26-a84c-84563052368d");
            var claim1 = TestDataHelper.CreateClaim().ToEntity();
            claim1.Id = new Guid("45a04de8-9a52-41d2-a09d-d3d6f5678c89");
            claim1.TargetId = targetId;
            claim1.CreatedAt = new DateTime(2022, 9, 5).ToUniversalTime();
            claim1.Document.FileSize = 0;
            claim1.Document.FileType = null;
            claim1.Document.UploadedAt = null;
            DatabaseContext.Add(claim1);
            DatabaseContext.Add(claim1.Document);

            var claim2 = TestDataHelper.CreateClaim().ToEntity();
            claim2.Id = new Guid("ae84084d-0c20-480f-a84e-e131a9a820bb");
            claim2.TargetId = targetId;
            claim2.CreatedAt = new DateTime(2021, 5, 23).ToUniversalTime();
            claim2.Document.FileSize = 0;
            claim2.Document.FileType = null;
            claim2.Document.UploadedAt = null;
            DatabaseContext.Add(claim2);
            DatabaseContext.Add(claim2.Document);

            var claim3 = TestDataHelper.CreateClaim().ToEntity();
            claim3.Id = new Guid("dffbe7a7-f27c-4ec4-bea3-40b57899c8dd");
            claim3.TargetId = targetId;
            claim3.CreatedAt = new DateTime(2020, 4, 18).ToUniversalTime();
            claim3.Document.FileSize = 0;
            claim3.Document.FileType = null;
            claim3.Document.UploadedAt = null;
            DatabaseContext.Add(claim3);
            DatabaseContext.Add(claim3.Document);
            DatabaseContext.SaveChanges();

            var uri = new Uri($"api/v1/claims?targetId={targetId}&limit=2&before=ew0KICAiaWQiOiJkZmZiZTdhNy1mMjdjLTRlYzQtYmVhMy00MGI1Nzg5OWM4ZGQiDQp9", UriKind.Relative);
            Client.DefaultRequestHeaders.Add("Authorization", TestToken.Value);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);
            var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var result = JsonConvert.DeserializeObject<PaginatedClaimResponse>(jsonString);

            var formattedCreatedAt1 = JsonConvert.SerializeObject(claim1.CreatedAt);
            var formattedDocumentCreatedAt1 = JsonConvert.SerializeObject(claim1.Document.CreatedAt);
            var formattedRetentionExpiresAt1 = JsonConvert.SerializeObject(claim1.RetentionExpiresAt);
            var formattedValidUntil1 = JsonConvert.SerializeObject(claim1.ValidUntil);

            var formattedCreatedAt2 = JsonConvert.SerializeObject(claim2.CreatedAt);
            var formattedDocumentCreatedAt2 = JsonConvert.SerializeObject(claim2.Document.CreatedAt);
            var formattedRetentionExpiresAt2 = JsonConvert.SerializeObject(claim2.RetentionExpiresAt);
            var formattedValidUntil2 = JsonConvert.SerializeObject(claim2.ValidUntil);

            string expected = "{" +
                            "\"claims\":[{" +
                            $"\"id\":\"{claim1.Id}\"," +
                              $"\"createdAt\":{formattedCreatedAt1}," +
                              "\"document\":{" +
                                  $"\"id\":\"{claim1.Document.Id}\"," +
                                  $"\"createdAt\":{formattedDocumentCreatedAt1}," +
                                  $"\"name\":\"{claim1.Document.Name}\"," +
                                  $"\"fileSize\":0," +
                                  $"\"fileType\":null," +
                                  $"\"uploadedAt\":null" +
                              "}," +
                              $"\"serviceAreaCreatedBy\":\"{claim1.ServiceAreaCreatedBy}\"," +
                              $"\"userCreatedBy\":\"{claim1.UserCreatedBy}\"," +
                              $"\"apiCreatedBy\":\"{claim1.ApiCreatedBy}\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt1}," +
                              $"\"validUntil\":{formattedValidUntil1}," +
                              $"\"targetId\":\"{claim1.TargetId}\"" +
                              "}," +
                              "{" +
                              $"\"id\":\"{claim2.Id}\"," +
                              $"\"createdAt\":{formattedCreatedAt2}," +
                              "\"document\":{" +
                                  $"\"id\":\"{claim2.Document.Id}\"," +
                                  $"\"createdAt\":{formattedDocumentCreatedAt2}," +
                                  $"\"name\":\"{claim2.Document.Name}\"," +
                                  $"\"fileSize\":0," +
                                  $"\"fileType\":null," +
                                  $"\"uploadedAt\":null" +
                              "}," +
                              $"\"serviceAreaCreatedBy\":\"{claim2.ServiceAreaCreatedBy}\"," +
                              $"\"userCreatedBy\":\"{claim2.UserCreatedBy}\"," +
                              $"\"apiCreatedBy\":\"{claim2.ApiCreatedBy}\"," +
                              $"\"retentionExpiresAt\":{formattedRetentionExpiresAt2}," +
                              $"\"validUntil\":{formattedValidUntil2}," +
                              $"\"targetId\":\"{claim2.TargetId}\"" +
                              "}" +
                            "]," +
                            "\"paging\":{" +
                                "\"cursors\":{" +
                                    "\"before\":\"ewogICJpZCI6ICI0NWEwNGRlOC05YTUyLTQxZDItYTA5ZC1kM2Q2ZjU2NzhjODkiCn0\"," +
                                    "\"after\":\"ewogICJpZCI6ICJhZTg0MDg0ZC0wYzIwLTQ4MGYtYTg0ZS1lMTMxYTlhODIwYmIiCn0\"" +
                                "}," +
                                "\"hasPreviousPage\":false," +
                                "\"hasNextPage\":true" +
                            "}" +
                        "}";

            response.StatusCode.Should().Be(200);
            jsonString.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task Returns400WhenTargetIdIsNotGuid()
        {
            var invalidTargetId = "abc";

            var uri = new Uri($"api/v1/claims?targetId={invalidTargetId}", UriKind.Relative);
            Client.DefaultRequestHeaders.Add("Authorization", TestToken.Value);

            var response = await Client.GetAsync(uri);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task Returns400WhenPreviousAndNextPageAreRequested()
        {
            var uri = new Uri($"api/v1/claims?targetId={Guid.NewGuid()}&before=aaa&after=bbb", UriKind.Relative);
            Client.DefaultRequestHeaders.Add("Authorization", TestToken.Value);

            var response = await Client.GetAsync(uri).ConfigureAwait(true);

            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task Returns401WhenNotAuthorizedToGetClaimsByTargetId()
        {
            var targetId = "c585f2d3-69c8-4a5e-b74f-c0570665c2d8";

            var uri = new Uri($"api/v1/claims?targetId={targetId}", UriKind.Relative);

            var response = await Client.GetAsync(uri);

            response.StatusCode.Should().Be(401);
        }
    }
}
