using SocialMedia.Domain.Primitives;
using SocialMedia.Domain.Users.Events;

namespace SocialMedia.Domain.Users;

public sealed class User : SoftDeletableEventfulEntity
{
    private User(
        Guid id,
        string clerkUserId,
        string? username,
        string? firstName,
        string? lastName,
        string? avatarUrl)
        : base(id)
    {
        ClerkUserId = clerkUserId;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
    }

    public string ClerkUserId { get; private set; }

    public string? Username { get; private set; }

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string? AvatarUrl { get; private set; }

    public static User Create(
        string clerkUserId,
        string? username,
        string? firstName,
        string? lastName,
        string? avatarUrl)
    {
        var user = new User(Guid.NewGuid(), clerkUserId, username, firstName, lastName, avatarUrl);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }

    public void UpdateProfile(string? username, string? firstName, string? lastName, string? avatarUrl)
    {
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
    }
}
