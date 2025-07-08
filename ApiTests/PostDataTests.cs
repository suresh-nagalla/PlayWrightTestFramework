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
    public class PostDataTests : ApiTestBase
    {
        [Test]
        public async Task TestPostData()
        {
            // Arrange
            var newItem = new DataItem
            {
                Name = "Test Item " + Guid.NewGuid(),
                Description = "Optional description"
            };

            var options = new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                Data = JsonSerializer.Serialize(newItem, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            };

            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(newItem, new JsonSerializerOptions { WriteIndented = true }));

            // Act
            var response = await API.PostAsync("/api/data", options);
            var responseBody = (JsonElement)await response.JsonAsync();

            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            // Assert
            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id' property.");
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name' property.");
            Assert.AreEqual(newItem.Name, nameProperty.GetString(), "Response 'name' does not match the request.");
        }
    }
}