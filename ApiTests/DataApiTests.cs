using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;
using Models;

namespace ApiTests
{
    [TestFixture]
    public class DataApiTests : UiTestBase
    {
        [Test]
        public async Task PostData_ShouldReturnCreatedItem()
        {
            // Arrange
            var payload = new DataItem
            {
                Name = "Test Item " + Guid.NewGuid(),
                Description = "Test Description"
            };

            var options = new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Data = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            };

            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));

            // Act
            var response = await API.PostAsync("/api/data", options);
            response.EnsureSuccessStatusCode();

            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            // Assert
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProp), "Response JSON does not contain 'name' property.");
            Assert.AreEqual(payload.Name, nameProp.GetString(), "The 'name' property does not match the payload.");

            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProp), "Response JSON does not contain 'id' property.");
            Assert.Greater(idProp.GetInt32(), 0, "The 'id' property is not a positive integer.");
        }

        [Test]
        public async Task GetData_ShouldReturnDataArray()
        {
            // Act
            var response = await API.GetAsync("/api/data");
            response.EnsureSuccessStatusCode();

            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            // Assert
            Assert.AreEqual(JsonValueKind.Array, responseBody.ValueKind, "Response JSON is not an array.");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response JSON array is empty.");
        }
    }
}