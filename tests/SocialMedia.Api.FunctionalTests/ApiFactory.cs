using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Api.Authentication;

namespace SocialMedia.Api.FunctionalTests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public const string Issuer = "https://clerk.test";
    public const string AuthorizedParty = "http://localhost:5173";

    private readonly RsaSecurityKey _signingKey = new(RSA.Create(2048)) { KeyId = "test-key" };

    public string CreateToken(string subject, string? authorizedParty = AuthorizedParty, bool expired = false)
    {
        var now = DateTime.UtcNow;
        var issuedAt = expired ? now.AddHours(-2) : now;
        var claims = new Dictionary<string, object> { [ClerkClaimTypes.Subject] = subject };

        if (authorizedParty is not null)
        {
            claims[ClerkClaimTypes.AuthorizedParty] = authorizedParty;
        }

        return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Claims = claims,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = issuedAt.AddMinutes(1),
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256),
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Configure<ClerkOptions>(options =>
            {
                options.Authority = Issuer;
                options.AuthorizedParties = [AuthorizedParty];
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var configuration = new OpenIdConnectConfiguration { Issuer = Issuer };
                configuration.SigningKeys.Add(_signingKey);

                options.Configuration = configuration;
                options.ConfigurationManager =
                    new StaticConfigurationManager<OpenIdConnectConfiguration>(configuration);
            });
        });
    }
}
