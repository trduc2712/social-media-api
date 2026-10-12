using SocialMedia.Domain.Primitives;
using SocialMedia.Domain.Users;
using SocialMedia.Domain.Users.Events;
using SocialMedia.Tests.Common.Users;

namespace SocialMedia.Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        var user = User.Create("user_123", "jane", "Jane", "Doe", "https://example.com/jane.png");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("user_123", user.ClerkUserId);
        Assert.Equal("jane", user.Username);
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("https://example.com/jane.png", user.AvatarUrl);
    }

    [Fact]
    public void UpdateProfile_ShouldChangeProfile()
    {
        var user = UserFactory.Create();

        user.UpdateProfile("john", "John", "Smith", null);

        Assert.Equal("john", user.Username);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Smith", user.LastName);
        Assert.Null(user.AvatarUrl);
    }

    [Fact]
    public void Create_ShouldRaiseUserCreatedDomainEvent()
    {
        var user = UserFactory.Create();

        var domainEvent = Assert.IsType<UserCreatedDomainEvent>(Assert.Single(user.DomainEvents));

        Assert.Equal(user.Id, domainEvent.UserId);
    }

    [Fact]
    public void User_ShouldBeAuditableAndSoftDeletable()
    {
        var user = UserFactory.Create();

        Assert.IsAssignableFrom<IAuditable>(user);
        Assert.IsAssignableFrom<ISoftDeletable>(user);
        Assert.False(user.IsDeleted);
    }
}
