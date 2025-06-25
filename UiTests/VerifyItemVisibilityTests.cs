using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;

namespace UiTests
{
    [TestFixture]
    public class VerifyItemVisibilityTests : UiTestBase
    {
        [Test]
        public async Task VerifyItemVisibility_ShouldHighlightAndCaptureScreenshot()
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
            await API.PostAsync("/api/data", options);

            // Act
            await Page.GotoAsync("http://localhost:5000");
            var itemSelector = $"li[data-item-id='{payload.Id}']";
            var item = await Page.QuerySelectorAsync(itemSelector);

            // Assert
            Assert.IsNotNull(item, "Item not found in the UI");
            var itemName = await item.EvalOnSelectorAsync("strong", "el => el.textContent");
            Assert.AreEqual(payload.Name, itemName, "Item name does not match");
            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");

            // Capture Screenshot
            var screenshotPath = $"screenshots/{payload.Id}.png";
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}
