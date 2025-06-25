using NUnit.Framework;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TestBase;

namespace ApiTests
{
    [TestFixture]
    public class PostDataTests : UiTestBase
    {
        [Test]
        public async Task PostData_ShouldReturnCreatedItem()
        {
            // Arrange
            var payload = new {
                id = Guid.NewGuid(),
                name = "Test Item " + Guid.NewGuid()
            };
            var options = new APIRequestContextOptions
            {
                Content = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            };

            // Act
            var response = await ApiRequestContext.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name'");
            Assert.AreEqual(payload.id.ToString(), idProperty.GetString(), "Incorrect 'id' value");
            Assert.AreEqual(payload.name, nameProperty.GetString(), "Incorrect 'name' value");

            // Log
            TestContext.WriteLine("Request Payload: " + JsonSerializer.Serialize(payload));
            TestContext.WriteLine("Response Body: " + responseBody);
        }
    }
}