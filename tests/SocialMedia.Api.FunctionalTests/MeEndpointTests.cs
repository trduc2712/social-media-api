using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SocialMedia.Contract.Users;

namespace SocialMedia.Api.FunctionalTests;

public class MeEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string Endpoint = "/api/v1/me";

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenTokenIsMissing()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync(Endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnCurrentUser_WhenTokenIsValid()
    {
        var client = CreateClient(factory.CreateToken("user_123"));

        var response = await client.GetAsync(Endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<CurrentUserResponse>(CancellationToken.None);
        Assert.Equal("user_123", user?.Id);
    }

    [Fact]
    public async Task Get_ShouldReturnCurrentUser_WhenTokenHasNoAuthorizedParty()
    {
        var client = CreateClient(factory.CreateToken("user_123", authorizedParty: null));

        var response = await client.GetAsync(Endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenAuthorizedPartyIsNotAllowed()
    {
        var client = CreateClient(factory.CreateToken("user_123", authorizedParty: "https://evil.example"));

        var response = await client.GetAsync(Endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenTokenIsExpired()
    {
        var client = CreateClient(factory.CreateToken("user_123", expired: true));

        var response = await client.GetAsync(Endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnUnauthorized_WhenTokenIsSignedByAnotherKey()
    {
        using var otherFactory = new ApiFactory();
        var client = CreateClient(otherFactory.CreateToken("user_123"));

        var response = await client.GetAsync(Endpoint, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private HttpClient CreateClient(string token)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
