using NUnit.Framework;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;
using Models;

namespace ApiTests
{
    public class DataApiTests : UiTestBase
    {
        [Test]
        public async Task PostDataItem_ShouldCreateNewItem()
        {
            // Arrange
            var payload = new CreateDataItemRequest
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

            // Act
            var response = await API.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name'");
            Assert.AreEqual(payload.Name, nameProperty.GetString(), "Name does not match");
        }

        [Test]
        public async Task GetDataItems_ShouldReturnItems()
        {
            // Act
            var response = await API.GetAsync("/api/data");
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            Assert.IsTrue(responseBody.ValueKind == JsonValueKind.Array, "Response is not an array");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty");
        }
    }
}