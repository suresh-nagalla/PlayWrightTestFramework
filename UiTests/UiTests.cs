using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaywrightSharp;
using TestBase;

namespace UiTests
{
    [TestClass]
    public class UiTests : UiTestBase
    {
        private readonly string _baseUrl = "http://localhost:5000";

        [TestMethod]
        public async Task TestVerifyNewItemInUI()
        {
            // Arrange
            var newItem = new DataItem { Id = Guid.NewGuid(), Name = "Test Item" };
            var jsonContent = new StringContent(JsonSerializer.Serialize(newItem), Encoding.UTF8, "application/json");
            await ApiRequestContext.PostAsync("http://localhost:5000/api/data", jsonContent);

            // Act
            await Page.GoToAsync(_baseUrl);
            var listItem = await Page.Locator($"li[data-item-id='{newItem.Id}']").FirstOrDefaultAsync();

            // Assert
            Assert.IsNotNull(listItem);
            var itemName = await listItem.InnerTextAsync();
            Assert.AreEqual(newItem.Name, itemName);
        }
    }
}
