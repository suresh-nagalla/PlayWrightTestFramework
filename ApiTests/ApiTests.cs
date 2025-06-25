using Microsoft.Playwright;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestBase;

namespace ApiTests;

public class ApiTests : UiTestBase
{
    [Test]
    public async Task Test_PostNewItem()
    {
        // Arrange
        var newItem = new { name = "TestItem" };

        // Act
        var response = await API.PostAsync("/api/data", new JsonContent(newItem));

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.Status);
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.IsNotNull(responseData);
        Assert.IsTrue(responseData.ContainsKey("id"));
    }

    [Test]
    public async Task Test_GetItems()
    {
        // Act
        var response = await API.GetAsync("/api/data");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.Status);
        var items = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        Assert.IsNotNull(items);
        Assert.IsNotEmpty(items);
    }
}
