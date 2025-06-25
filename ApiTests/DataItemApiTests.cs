using NUnit.Framework;
using Microsoft.Playwright;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TestBase;
using Models;

namespace ApiTests;

[TestFixture]
public class DataItemApiTests : UiTestBase
{
    [Test]
    public async Task Test_PostDataItem()
    {
        // Arrange
        var payload = new CreateDataItemRequest
        {
            Name = "Test Item " + Guid.NewGuid(),
            Description = "Test Description"
        };

        var options = new APIRequestContextOptions
        {
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }
            },
            Data = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        };

        // Act
        var response = await API.PostAsync("/api/data", options);
        JsonElement responseBody = (JsonElement)await response.JsonAsync();

        // Assert
        Assert.IsTrue(responseBody.TryGetProperty("id", out var idProperty), "Response does not contain 'id'");
        Assert.IsTrue(responseBody.TryGetProperty("name", out var nameProperty), "Response does not contain 'name'");
        Assert.AreEqual(payload.Name, nameProperty.GetString(), "Name does not match");

        TestContext.WriteLine("Request: " + JsonSerializer.Serialize(payload));
        TestContext.WriteLine("Response: " + responseBody.ToString());
    }

    [Test]
    public async Task Test_GetDataItems()
    {
        // Act
        var response = await API.GetAsync("/api/data");
        JsonElement responseBody = (JsonElement)await response.JsonAsync();

        // Assert
        Assert.IsTrue(responseBody.ValueKind == JsonValueKind.Array, "Response is not an array");
        Assert.IsTrue(responseBody.GetArrayLength() > 0, "Response array is empty");

        TestContext.WriteLine("Response: " + responseBody.ToString());
    }
}
