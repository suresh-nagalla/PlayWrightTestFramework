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
        public async Task CreatedItem_ShouldBeVisibleInUI()
        {
            // Arrange
            var payload = new {
                id = Guid.NewGuid(),
                name = "Test Item " + Guid.NewGuid()
            };
            var options = new APIRequestContextOptions
            {
                Content = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            };
            var response = await ApiRequestContext.PostAsync("/api/data", options);
            JsonElement responseBody = (JsonElement)await response.JsonAsync();
            var itemId = responseBody.GetProperty("id").GetString();
            var itemName = responseBody.GetProperty("name").GetString();

            // Act
            await Page.GotoAsync("/");
            var itemSelector = $"li[data-item-id='{itemId}']";
            var itemElement = await Page.QuerySelectorAsync(itemSelector);

            // Assert
            Assert.IsNotNull(itemElement, "Item element not found in UI");
            Assert.IsTrue(await itemElement.IsVisibleAsync(), "Item element is not visible");
            var innerText = await itemElement.InnerTextAsync();
            Assert.IsTrue(innerText.Contains(itemName), "Item element does not contain the correct name");

            // Highlight and Screenshot
            await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");
            var screenshotPath = System.IO.Path.Combine(TestContext.CurrentContext.WorkDirectory, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath);
        }
    }
}