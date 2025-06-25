using NUnit.Framework;
using System.Threading.Tasks;
using TestBase;

namespace UiTests;

public class UiTests : UiTestBase
{
    [Test]
    public async Task TestVerifyNewItemInUI()
    {
        // Arrange
        var newItem = new { name = "Test Item" };
        var response = await API.PostAsync("/api/data", newItem);
        var responseBody = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var itemId = responseBody["id"].ToString();

        // Act
        await Page.GotoAsync("http://localhost:5000");
        var listItem = Page.Locator($"li[data-item-id='{itemId}']");

        // Assert
        Assert.IsTrue(await listItem.IsVisibleAsync());
        Assert.AreEqual(newItem.name, await listItem.TextContentAsync());
    }
}