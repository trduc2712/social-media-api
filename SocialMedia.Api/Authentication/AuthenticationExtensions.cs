using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Abstractions;

namespace SocialMedia.Api.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddClerkAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ClerkOptions>()
            .Bind(configuration.GetSection(ClerkOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Authority),
                $"{ClerkOptions.SectionName}:{nameof(ClerkOptions.Authority)} is required.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<ClerkOptions>>((options, clerkOptions) =>
            {
                var clerk = clerkOptions.Value;

                options.Authority = clerk.Authority;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = ClerkClaimTypes.Subject;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var authorizedParty = context.Principal?.FindFirst(ClerkClaimTypes.AuthorizedParty)?.Value;

                        if (authorizedParty is not null && !clerk.AuthorizedParties.Contains(authorizedParty))
                        {
                            context.Fail("The token was issued for an unauthorized party.");
                        }

                        return Task.CompletedTask;
                    },
                };
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
