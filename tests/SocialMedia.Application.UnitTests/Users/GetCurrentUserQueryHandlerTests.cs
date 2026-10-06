using SocialMedia.Application.Abstractions;
using SocialMedia.Application.Users.GetCurrentUser;

namespace SocialMedia.Application.UnitTests.Users;

public class GetCurrentUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCurrentUserId()
    {
        var handler = new GetCurrentUserQueryHandler(new FakeCurrentUser("user_123"));

        var response = await handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        Assert.Equal("user_123", response.Id);
    }

    private sealed class FakeCurrentUser(string id) : ICurrentUser
    {
        public string Id { get; } = id;
    }
}
