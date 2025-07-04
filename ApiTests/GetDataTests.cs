using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TestBase;

namespace ApiTests
{
    [TestFixture]
    public class GetDataTests : UiTestBase
    {
        [Test]
        public async Task GetData_ShouldReturnDataArray()
        {
            // Arrange
            // No arrangement needed for GET request

            // Act
            var response = await API.GetAsync("/api/data");
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());
            Assert.IsTrue(responseBody.ValueKind == JsonValueKind.Array, "Response is not an array.");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty.");
        }
    }
}
