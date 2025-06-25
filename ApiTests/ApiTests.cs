using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using TestBase;

namespace ApiTests
{
    [TestClass]
    public class ApiTests : UiTestBase
    {
        private readonly string _baseUrl = "http://localhost:5000/api/data";

        [TestMethod]
        public async Task TestPostData()
        {
            // Arrange
            var newItem = new DataItem { Id = Guid.NewGuid(), Name = "Test Item" };
            var jsonContent = new StringContent(JsonSerializer.Serialize(newItem), Encoding.UTF8, "application/json");

            // Act
            var response = await ApiRequestContext.PostAsync(_baseUrl, jsonContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var responseData = await response.Content.ReadAsStringAsync();
            var createdItem = JsonSerializer.Deserialize<DataItem>(responseData);
            Assert.AreEqual(newItem.Name, createdItem.Name);
        }

        [TestMethod]
        public async Task TestGetData()
        {
            // Arrange
            var response = await ApiRequestContext.GetAsync(_baseUrl);

            // Act
            var responseData = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<List<DataItem>>(responseData);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(items.Count > 0);
        }
    }
}
