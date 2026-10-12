using SocialMedia.Domain.Users;

namespace SocialMedia.Tests.Common.Users;

public static class UserFactory
{
    public static User Create(
        string clerkUserId = "user_123",
        string? username = "jane",
        string? firstName = "Jane",
        string? lastName = "Doe",
        string? avatarUrl = "https://example.com/jane.png") =>
        User.Create(clerkUserId, username, firstName, lastName, avatarUrl);
}
