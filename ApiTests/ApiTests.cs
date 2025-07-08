using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Json;

namespace ApiTests
{
    public class ApiTests : TestBase.UiTestBase
    {
        [Test]
        public async Task Test_Post_NewItem_ShouldReturnCreated()
        {
            // Arrange
            var newItem = new { Name = "Test Item" };

            // Act
            var response = await API.PostAsync("/api/data", newItem);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.Status);
            var responseData = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.AreEqual(newItem.Name, responseData.name.ToString());
        }

        [Test]
        public async Task Test_Get_AllItems_ShouldReturnOk()
        {
            // Act
            var response = await API.GetAsync("/api/data");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            var responseData = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.IsNotNull(responseData);
        }
    }
}
