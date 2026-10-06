using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Api.Authentication;
using SocialMedia.Api.OpenApi;

namespace SocialMedia.Api.FunctionalTests;

public class OpenApiEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string DocumentEndpoint = "/openapi/v1.json";
    private const string SwaggerEndpoint = "/swagger/index.html";
    private const string MePath = "/api/v1/me";

    [Fact]
    public async Task GetDocument_ShouldReturnOk_WhenTokenIsMissing()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(DocumentEndpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDocument_ShouldDescribeTitleAndVersion()
    {
        using var document = await GetDocumentAsync();

        var info = document.RootElement.GetProperty("info");
        Assert.Equal("Social Media API", info.GetProperty("title").GetString());
        Assert.Equal("1.0", info.GetProperty("version").GetString());
    }

    [Fact]
    public async Task GetDocument_ShouldDeclareBearerSecurityScheme()
    {
        using var document = await GetDocumentAsync();

        var scheme = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty(BearerSecurityTransformer.SchemeName);
        Assert.Equal("http", scheme.GetProperty("type").GetString());
        Assert.Equal("bearer", scheme.GetProperty("scheme").GetString());
    }

    [Fact]
    public async Task GetDocument_ShouldRequireBearerToken_ForProtectedOperation()
    {
        using var document = await GetDocumentAsync();

        var operation = document.RootElement.GetProperty("paths").GetProperty(MePath).GetProperty("get");
        Assert.Contains(
            operation.GetProperty("security").EnumerateArray(),
            requirement => requirement.TryGetProperty(BearerSecurityTransformer.SchemeName, out _));
        Assert.True(operation.GetProperty("responses").TryGetProperty("200", out _));
        Assert.True(operation.GetProperty("responses").TryGetProperty("401", out _));
        Assert.True(operation.GetProperty("responses").TryGetProperty("403", out _));
    }

    [Fact]
    public async Task GetSwaggerUi_ShouldReturnOk_WhenTokenIsMissing()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(SwaggerEndpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("pk_test_example", true)]
    [InlineData("", false)]
    public async Task GetSwaggerUi_ShouldOfferClerkLogin_OnlyWhenPublishableKeyIsConfigured(
        string publishableKey,
        bool expected)
    {
        using var clerkFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                services.Configure<ClerkOptions>(options => options.PublishableKey = publishableKey)));
        var client = clerkFactory.CreateClient();

        var page = await client.GetStringAsync(SwaggerEndpoint, CancellationToken.None);
        var script = await client.GetStringAsync("/swagger/index.js", CancellationToken.None);

        Assert.Equal(expected, page.Contains("data-clerk-publishable-key=\"pk_test_example\"", StringComparison.Ordinal));
        Assert.Equal(expected, page.Contains("window.attachClerkToken", StringComparison.Ordinal));
        Assert.Equal(expected, page.Contains($"{ApiFactory.Issuer}/npm/@clerk/clerk-js", StringComparison.Ordinal));
        Assert.Equal(expected, script.Contains("ClerkAuthPlugin", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(DocumentEndpoint)]
    [InlineData(SwaggerEndpoint)]
    public async Task Get_ShouldReturnNotFound_WhenDocumentationIsDisabled(string endpoint)
    {
        using var disabledFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
                services.Configure<ApiDocumentationOptions>(options => options.Enabled = false)));
        var client = disabledFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", factory.CreateToken("user_123"));

        var response = await client.GetAsync(endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReportSupportedVersions_ForVersionedEndpoint()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", factory.CreateToken("user_123"));

        var response = await client.GetAsync(MePath, CancellationToken.None);

        Assert.Equal("1.0", Assert.Single(response.Headers.GetValues("api-supported-versions")));
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenVersionIsMissingFromUrl()
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", factory.CreateToken("user_123"));

        var response = await client.GetAsync("/api/me", CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<JsonDocument> GetDocumentAsync()
    {
        var client = factory.CreateClient();
        await using var stream = await client.GetStreamAsync(DocumentEndpoint, CancellationToken.None);

        return await JsonDocument.ParseAsync(stream, cancellationToken: CancellationToken.None);
    }
}
