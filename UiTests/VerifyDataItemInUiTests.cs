using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Models;
using TestBase;

namespace UiTests
{
    public class VerifyDataItemInUiTests : UiTestBase
    {
        [Test]
        public async Task VerifyDataItemInUi_ShouldHighlightItem()
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

            var response = await API.PostAsync("/api/data", options);
            var responseBody = (JsonElement)await response.JsonAsync();

            responseBody.TryGetProperty("id", out var idProperty);
            var itemId = idProperty.GetInt32();

            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(newItem, new JsonSerializerOptions { WriteIndented = true }));
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            // Act
            TestContext.WriteLine("🌐 Navigating to UI: http://localhost:5000");
            await Page.GotoAsync("http://localhost:5000");

            var selector = $"li[data-item-id='{itemId}']";
            TestContext.WriteLine($"🔍 Verifying item with selector: {selector}");
            var item = await Page.QuerySelectorAsync(selector);

            // Assert
            Assert.IsNotNull(item, $"Item with id {itemId} was not found.");

            var itemName = await item.EvaluateAsync<string>("el => el.querySelector('strong').innerText");
            Assert.AreEqual(newItem.Name, itemName, "The name of the item in the UI does not match");

            await Page.EvalOnSelectorAsync(selector, "el => el.style.border = '3px solid red'");

            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.WriteLine($"📸 Screenshot saved at: {screenshotPath}");
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}
