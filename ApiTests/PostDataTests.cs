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
        public async Task PostData_ShouldReturnSuccess()
        {
            // Arrange
            var payload = new {
                Id = Guid.NewGuid(),
                Name = "Test Item"
            };
            var options = new APIRequestContextOptions
            {
                ContentType = "application/json",
                Data = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            };

            // Act
            var response = await API.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
            Assert.AreEqual(payload.Id.ToString(), idProperty.GetString(), "ID does not match");
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name'");
            Assert.AreEqual(payload.Name, nameProperty.GetString(), "Name does not match");

            // Log
            TestContext.WriteLine($"Request: {JsonSerializer.Serialize(payload)}");
            TestContext.WriteLine($"Response: {responseBody}");
        }
    }
}
