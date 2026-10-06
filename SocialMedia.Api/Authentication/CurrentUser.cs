using SocialMedia.Application.Abstractions;

namespace SocialMedia.Api.Authentication;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id =>
        httpContextAccessor.HttpContext?.User.FindFirst(ClerkClaimTypes.Subject)?.Value
        ?? throw new InvalidOperationException("The current request has no authenticated user.");
}
