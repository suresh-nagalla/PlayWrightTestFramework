using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;
using Models;

namespace UiTests;

[TestFixture]
public class DataItemUiTests : UiTestBase
{
    [Test]
    public async Task Test_VerifyCreatedItemIsVisible()
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

        var postResponse = await API.PostAsync("/api/data", options);
        JsonElement postResponseBody = (JsonElement)await postResponse.JsonAsync();

        Assert.IsTrue(postResponseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
        var itemId = idProperty.GetInt32();

        // Act
        await Page.GotoAsync("http://localhost:5000");
        var itemSelector = $"li[data-item-id='{itemId}']";
        var item = await Page.QuerySelectorAsync(itemSelector);

        // Assert
        Assert.IsNotNull(item, "Item not found in the UI");
        var itemName = await item.EvalOnSelectorAsync<string>("strong", "el => el.innerText");
        Assert.AreEqual(payload.Name, itemName, "Item name does not match");

        await Page.EvalOnSelectorAsync(itemSelector, "el => el.style.border = '3px solid red'");
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = "screenshot.png", FullPage = true });
        TestContext.AddTestAttachment("screenshot.png");
    }
}
