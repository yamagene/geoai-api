using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GeoaiApi.Tests;

/// <summary>OpenAPI と Scalar は開発環境でのみ公開する（spec.md 4.3）。</summary>
public class OpenApiExposureTests
{
    [Theory]
    [InlineData("Development", HttpStatusCode.OK)]
    [InlineData("Production", HttpStatusCode.NotFound)]
    public async Task OpenApiAndScalar_OnlyInDevelopment(string environment, HttpStatusCode expected)
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseEnvironment(environment));
        using var client = factory.CreateClient();

        Assert.Equal(expected, (await client.GetAsync("/openapi/v1.json")).StatusCode);
        Assert.Equal(expected, (await client.GetAsync("/scalar")).StatusCode);
    }
}
