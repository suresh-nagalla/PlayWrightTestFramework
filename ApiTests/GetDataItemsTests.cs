using NUnit.Framework;
using System.Text.Json;
using System.Threading.Tasks;
using TestBase;

namespace ApiTests
{
    public class GetDataItemsTests : UiTestBase
    {
        [Test]
        public async Task GetDataItems_ShouldReturnNonEmptyArray()
        {
            // Act
            var response = await API.GetAsync("/api/data");
            var responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.IsTrue(responseBody.ValueKind == JsonValueKind.Array, "Response is not an array");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty");

            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());
        }
    }
}
