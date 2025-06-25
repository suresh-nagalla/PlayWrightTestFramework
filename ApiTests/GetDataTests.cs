using NUnit.Framework;
using System.Text.Json;
using System.Threading.Tasks;
using TestBase;

namespace ApiTests
{
    [TestFixture]
    public class GetDataTests : UiTestBase
    {
        [Test]
        public async Task GetData_ShouldReturnNonEmptyArray()
        {
            // Act
            var response = await ApiRequestContext.GetAsync("/api/data");
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.AreEqual(JsonValueKind.Array, responseBody.ValueKind, "Response is not an array");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty");
            var firstElement = responseBody[0];
            Assert.IsTrue(firstElement.TryGetProperty("id", out var idProperty), "First element does not contain 'id'");
            Assert.IsTrue(firstElement.TryGetProperty("name", out var nameProperty), "First element does not contain 'name'");

            // Log
            TestContext.WriteLine("Response Body: " + responseBody);
        }
    }
}