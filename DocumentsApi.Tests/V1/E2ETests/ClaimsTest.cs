using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    }
}
