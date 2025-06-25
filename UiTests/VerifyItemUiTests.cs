using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;
using Models;

namespace UiTests
{
    public class VerifyItemUiTests : UiTestBase
    {
        [Test]
        public async Task CreatedItem_ShouldBeVisibleInUI()
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

            var response = await API.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
            var itemId = idProperty.GetInt32();
            var itemName = payload.Name;

            // Act
            await Page.GotoAsync("http://localhost:5000");
            TestContext.WriteLine("🌐 Navigating to UI: http://localhost:5000");

            var itemSelector = $"li[data-item-id='{itemId}']";
            var itemElement = await Page.QuerySelectorAsync(itemSelector);

            // Assert
            TestContext.WriteLine($"🔍 Verifying item with selector: {itemSelector}");
            Assert.IsNotNull(itemElement, "Item not found in UI");

            var itemNameElement = await itemElement.QuerySelectorAsync("strong");
            Assert.IsNotNull(itemNameElement, "Item name element not found");

            var itemNameText = await itemNameElement.InnerTextAsync();
            Assert.AreEqual(itemName, itemNameText, "Item name does not match");

            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");
            var screenshotPath = $"screenshot_{itemId}.png";
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
TestContext.WriteLine($"📸 Screenshot captured: {screenshotPath}");
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}