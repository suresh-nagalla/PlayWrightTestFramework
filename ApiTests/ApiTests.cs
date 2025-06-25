using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestBase;

namespace ApiTests;

public class ApiTests : UiTestBase
{
    [Test]
    public async Task TestPostNewItem()
    {
        // Arrange
        var newItem = new { name = "Test Item" };

        // Act
        var response = await API.PostAsync("/api/data", newItem);
        var responseBody = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(responseBody["id"]);
        Assert.AreEqual(newItem.name, responseBody["name"]);
    }

    [Test]
    public async Task TestGetItems()
    {
        // Arrange
        var response = await API.GetAsync("/api/data");

        // Act
        var responseBody = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotEmpty(responseBody);
    }
}