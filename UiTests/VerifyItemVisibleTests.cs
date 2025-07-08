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
    public class VerifyItemVisibleTests : UiTestBase
    {
        [Test]
        public async Task TestItemIsVisibleInUI()
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

            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id' property.");
            var itemId = idProperty.GetInt32();

            // Navigate to the home page
            TestContext.WriteLine("🌐 Navigating to UI: http://localhost:5000");
            await Page.GotoAsync("http://localhost:5000");

            // Verify the item appears in the UI
            var selector = $"li[data-item-id='{itemId}']";
            TestContext.WriteLine($"🔍 Verifying item with selector: {selector}");
            var item = await Page.QuerySelectorAsync(selector);

            Assert.IsNotNull(item, $"Item with id {itemId} was not found.");
            Assert.IsTrue(await item.IsVisibleAsync(), $"Item with id {itemId} is not visible.");
            Assert.AreEqual(newItem.Name, await item.InnerTextAsync(), $"Item name does not match.");

            // Highlight the found element
            await Page.EvalOnSelectorAsync(selector, "el => el.style.border = '3px solid red'");

            // Capture a full-page screenshot
            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.WriteLine($"📸 Screenshot saved at: {screenshotPath}");
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}