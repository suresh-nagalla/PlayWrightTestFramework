using Microsoft.Playwright;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using TestBase;

namespace UiTests;

public class UiTests : UiTestBase
{
    [Test]
    public async Task Test_VerifyNewItemInUI()
    {
        // Arrange
        var newItem = new { name = "UITestItem" };
        var response = await API.PostAsync("/api/data", new JsonContent(newItem));
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var newItemId = responseData["id"].ToString();

        // Act
        await Page.GotoAsync("http://localhost:5000");
        var listItem = Page.Locator($"li[data-item-id='{newItemId}']");

        // Assert
        Assert.IsTrue(await listItem.IsVisibleAsync());
        var itemName = await listItem.Locator("strong").InnerTextAsync();
        Assert.AreEqual("UITestItem", itemName);
    }
}
