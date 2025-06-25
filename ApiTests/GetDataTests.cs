using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;

namespace ApiTests
{
    [TestFixture]
    public class GetDataTests : UiTestBase
    {
        [Test]
        public async Task GetData_ShouldReturnDataArray()
        {
            // Act
            var response = await API.GetAsync("/api/data");
            JsonElement responseBody = (JsonElement)await response.JsonAsync();

            // Assert
            Assert.IsTrue(responseBody.ValueKind == JsonValueKind.Array, "Response is not an array");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty");

            // Log
            TestContext.WriteLine($"Response: {responseBody}");
        }
    }
}
