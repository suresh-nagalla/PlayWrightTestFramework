using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TestBase;

namespace UiTests
{
    [TestFixture]
    public class VerifyCreatedItemVisibleTests : UiTestBase
    {
        [Test]
        public async Task VerifyCreatedItemVisible_ShouldHighlightItem()
        {
            // Arrange
            var payload = new { id = Guid.NewGuid(), name = "Test Item" };
            var options = new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Data = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            };
            var postResponse = await ApiRequestContext.PostAsync("/api/data", options);
            JsonElement postResponseBody = (JsonElement)await postResponse.JsonAsync();

            // Act
            await Page.GotoAsync("/");
            var itemSelector = $"li[data-item-id='{payload.id}']";
            var item = await Page.QuerySelectorAsync(itemSelector);

            // Assert
            Assert.IsNotNull(item, "Item not found in the UI");
            Assert.IsTrue(await item.IsVisibleAsync(), "Item is not visible");
            var innerText = await item.InnerTextAsync();
            Assert.IsTrue(innerText.Contains(payload.name), "Item does not contain the correct name");

            // Highlight
            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");

            // Screenshot
            var screenshotPath = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath);

            // Log
            TestContext.WriteLine("Request Payload: " + JsonSerializer.Serialize(payload));
            TestContext.WriteLine("Post Response Body: " + postResponseBody.ToString());
            TestContext.WriteLine("Screenshot saved at: " + screenshotPath);
        }
    }
}