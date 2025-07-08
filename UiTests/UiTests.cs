using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;

namespace UiTests
{
    public class UiTests : TestBase.UiTestBase
    {
        [Test]
        public async Task Test_AddNewItemAndVerifyInUI()
        {
            // Arrange
            var newItem = new { Name = "Test Item" };
            var postResponse = await API.PostAsync("/api/data", newItem);
            var responseData = await postResponse.Content.ReadFromJsonAsync<dynamic>();
            string itemId = responseData.id.ToString();

            // Act
            await Page.GotoAsync("http://localhost:5000");
            var itemLocator = Page.Locator($"li[data-item-id='{itemId}']");

            // Assert
            Assert.IsTrue(await itemLocator.IsVisibleAsync(), "Newly added item is not visible in the UI");
        }
    }
}
