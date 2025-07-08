using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Models;
using TestBase;

namespace ApiTests
{
    public class PostDataItemTests : UiTestBase
    {
        [Test]
        public async Task PostDataItem_ShouldReturnCreatedItem()
        {
            // Arrange
            var newItem = new CreateDataItemRequest
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

            // Act
            var response = await API.PostAsync("/api/data", options);
            var responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name'");
            Assert.AreEqual(newItem.Name, nameProperty.GetString(), "The name of the created item does not match");

            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(newItem, new JsonSerializerOptions { WriteIndented = true }));
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());
        }
    }
}
