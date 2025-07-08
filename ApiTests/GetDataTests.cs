using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;

namespace ApiTests
{
    public class GetDataTests : ApiTestBase
    {
        [Test]
        public async Task TestGetData()
        {
            // Act
            var response = await API.GetAsync("/api/data");
            var responseBody = (JsonElement)await response.JsonAsync();

            TestContext.WriteLine("📥 Response Body:\n" + responseBody.ToString());

            // Assert
            Assert.IsTrue(responseBody.ValueKind == JsonValueKind.Array, "Response is not an array.");
            Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty.");
        }
    }
}