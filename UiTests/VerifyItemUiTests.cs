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

            TestContext.WriteLine("📤 Request Payload:\n" + JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));

            // Act
            var response = await API.PostAsync("/api/data", options);
            response.EnsureSuccessStatusCode();

            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            // Extract ID from response
            Assert.IsTrue(responseBody.TryGetProperty("id", out var idProp), "Response JSON does not contain 'id' property.");
            var itemId = idProp.GetInt32();

            // Navigate to UI
            TestContext.WriteLine("🌐 Navigating to UI: http://localhost:5000");
            await Page.GotoAsync("http://localhost:5000");

            // Assert item is visible in UI
            var itemSelector = $"li[data-item-id='{itemId}']";
            TestContext.WriteLine($"🔍 Verifying item with selector: {itemSelector}");
            var itemElement = await Page.QuerySelectorAsync(itemSelector);
            Assert.IsNotNull(itemElement, "Item not found in UI.");

            var itemName = await itemElement.EvalOnSelectorAsync<string>("strong", "el => el.textContent");
            Assert.AreEqual(payload.Name, itemName, "The item name in the UI does not match the payload.");

            // Highlight the item
            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");

            // Capture screenshot
            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.WriteLine($"📸 Screenshot saved at: {screenshotPath}");
            TestContext.WriteLine("✅ UI verification successful.");
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}