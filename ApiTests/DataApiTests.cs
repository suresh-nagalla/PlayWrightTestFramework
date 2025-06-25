using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;

namespace ApiTests
{
    [TestFixture]
    public class DataApiTests
    {
        private IAPIRequestContext _apiContext;

        [SetUp]
        public void Setup()
        {
            var playwright = Playwright.CreateAsync().Result;
            _apiContext = playwright.APIRequest.NewContextAsync().Result;
        }

        [Test]
        public async Task PostData_ShouldReturnSuccess()
        {
            // Arrange
            var payload = new { name = "Test Item " + Guid.NewGuid() };
            var options = new APIRequestContextOptions
            {
                ContentType = "application/json",
                Data = JsonSerializer.Serialize(payload)
            };

            // Act
            var response = await _apiContext.PostAsync("http://localhost:5000/api/data", options);
            var responseBody = await response.JsonAsync();

            // Assert
            Assert.AreEqual(201, response.Status);
            Assert.IsTrue(responseBody.TryGetProperty("id", out _));
            Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty));
            Assert.AreEqual(payload.name, nameProperty.GetString());

            // Log
            TestContext.WriteLine("Request Payload: " + JsonSerializer.Serialize(payload));
            TestContext.WriteLine("Response Body: " + responseBody);
        }

        [Test]
        public async Task GetData_ShouldReturnDataArray()
        {
            // Act
            var response = await _apiContext.GetAsync("http://localhost:5000/api/data");
            var responseBody = await response.JsonAsync();

            // Assert
            Assert.AreEqual(200, response.Status);
            Assert.AreEqual(JsonValueKind.Array, responseBody.ValueKind);
            Assert.IsTrue(responseBody[0].TryGetProperty("id", out _));
            Assert.IsTrue(responseBody[0].TryGetProperty("name", out _));
        }
    }
}