using NUnit.Framework;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;
using Models;
using System;
using Microsoft.Playwright;

namespace UiTests
{
    [TestFixture]
    public class DataUiTests : UiTestBase
    {
        [Test]
        public async Task Test_VerifyCreatedItemIsVisible()
        {
            // Arrange
            var payload = new CreateDataItemRequest
            {
                Name = "Test Item " + Guid.NewGuid(),
                Description = "This is a test description."
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

            var response = await API.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
            var itemId = idProperty.GetInt32();

            // Act
            await Page.GotoAsync("http://localhost:5000");
            TestContext.WriteLine("🌐 Navigating to UI: http://localhost:5000");

            // Assert
            var itemSelector = $"li[data-item-id='{itemId}']";
            var itemElement = await Page.QuerySelectorAsync(itemSelector);
            Assert.IsNotNull(itemElement, $"Item with ID {itemId} not found in the UI");

            var itemNameElement = await itemElement.QuerySelectorAsync("strong");
            Assert.IsNotNull(itemNameElement, "Item name element not found");
            var itemName = await itemNameElement.InnerTextAsync();
            Assert.AreEqual(payload.Name, itemName, "Item name does not match");

            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");
            var screenshotPath = $"screenshot_{itemId}.png";
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}
