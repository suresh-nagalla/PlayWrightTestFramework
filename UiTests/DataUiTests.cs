using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace UiTests
{
    [TestFixture]
    public class DataUiTests : UiTestBase
    {
        [Test]
        public async Task CreatedItem_ShouldBeVisibleInUI()
        {
            // Arrange
            var payload = new { name = "Test Item " + Guid.NewGuid() };
            var options = new APIRequestContextOptions
            {
                ContentType = "application/json",
                Data = JsonSerializer.Serialize(payload)
            };

            var response = await ApiContext.PostAsync("http://localhost:5000/api/data", options);
            var responseBody = await response.JsonAsync();
            var itemId = responseBody.GetProperty("id").GetString();

            // Act
            await Page.GotoAsync("http://localhost:5000");
            var itemSelector = $"li[data-item-id='{itemId}']";
            var itemElement = await Page.QuerySelectorAsync(itemSelector);

            // Assert
            Assert.IsNotNull(itemElement);
            Assert.IsTrue(await itemElement.IsVisibleAsync());
            var innerText = await itemElement.InnerTextAsync();
            Assert.IsTrue(innerText.Contains(payload.name));

            // Highlight
            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");

            // Screenshot
            var screenshotPath = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });

            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}