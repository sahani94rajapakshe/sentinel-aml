using System.Net;
using SentinelAML.IntegrationTests.WebApplicationFactory;

namespace SentinelAML.IntegrationTests;

public class HealthEndpointTests : IClassFixture<SentinelAMLWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(SentinelAMLWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
