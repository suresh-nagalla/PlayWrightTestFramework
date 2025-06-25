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
    [TestFixture]
    public class VerifyItemUiTests : UiTestBase
    {
        [Test]
        public async Task CreatedItem_ShouldBeVisibleInUI()
        {
            // Arrange
            var payload = new DataItem
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
                Data = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            };

            var response = await API.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();
            var itemId = responseBody.GetProperty("id").GetInt32();

            // Act
            await Page.GotoAsync("http://localhost:5000");
            TestContext.WriteLine("🌐 Navigating to UI: http://localhost:5000");

            var itemSelector = $"li[data-item-id='{itemId}']";
            var itemElement = await Page.QuerySelectorAsync(itemSelector);
            TestContext.WriteLine($"🔍 Verifying item with selector: {itemSelector}");

            // Assert
            Assert.NotNull(itemElement, "Item element not found in UI.");
            var itemName = await itemElement.EvalOnSelectorAsync("strong", "el => el.innerText");
            Assert.AreEqual(payload.Name, itemName.ToString(), "Item name does not match.");

            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");

            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.WriteLine($"📸 Screenshot saved at: {screenshotPath}");
            TestContext.WriteLine("✅ UI verification successful.");
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}