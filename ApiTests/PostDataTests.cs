using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;

namespace ApiTests
{
    [TestFixture]
    public class PostDataTests : UiTestBase
    {
        [Test]
        public async Task PostData_ShouldCreateNewItem()
        {
            // Arrange
            var payload = new {
                Name = "Test Item " + Guid.NewGuid(),
                Description = "Optional description"
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

            // Act
            var response = await API.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name' property.");
            Assert.AreEqual(payload.Name, nameProperty.GetString(), "Item name does not match.");
            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id' property.");
            Assert.Greater(idProperty.GetInt32(), 0, "Item ID is not valid.");

            // Log
            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());
        }
    }
}
