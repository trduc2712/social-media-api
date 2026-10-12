using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Domain.Users;
using SocialMedia.Tests.Common.Users;

namespace SocialMedia.Infrastructure.IntegrationTests.Users;

public class UserPersistenceTests(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 1, 1, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ChangedAt = new(2026, 1, 2, 9, 30, 0, TimeSpan.Zero);

    [Fact]
    public async Task SaveChanges_ShouldPersistUserAndStampCreatedAt()
    {
        var user = await AddUserAsync();

        var saved = await FindAsync(user.Id);

        Assert.NotNull(saved);
        Assert.Equal(user.ClerkUserId, saved.ClerkUserId);
        Assert.Equal("jane", saved.Username);
        Assert.Equal("Jane", saved.FirstName);
        Assert.Equal("Doe", saved.LastName);
        Assert.Equal("https://example.com/jane.png", saved.AvatarUrl);
        Assert.Equal(CreatedAt.UtcDateTime, saved.CreatedAtUtc);
        Assert.Null(saved.UpdatedAtUtc);
        Assert.False(saved.IsDeleted);
        Assert.Null(saved.DeletedAtUtc);
    }

    [Fact]
    public async Task SaveChanges_ShouldStampUpdatedAt_WhenUserChanges()
    {
        var user = await AddUserAsync();

        fixture.TimeProvider.UtcNow = ChangedAt;

        using (var scope = fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tracked = await dbContext.Users.SingleAsync(candidate => candidate.Id == user.Id);

            tracked.UpdateProfile("john", "John", "Smith", null);

            await dbContext.SaveChangesAsync();
        }

        var saved = await FindAsync(user.Id);

        Assert.NotNull(saved);
        Assert.Equal("john", saved.Username);
        Assert.Null(saved.AvatarUrl);
        Assert.Equal(CreatedAt.UtcDateTime, saved.CreatedAtUtc);
        Assert.Equal(ChangedAt.UtcDateTime, saved.UpdatedAtUtc);
    }

    [Fact]
    public async Task Remove_ShouldSoftDeleteUser()
    {
        var user = await AddUserAsync();

        fixture.TimeProvider.UtcNow = ChangedAt;

        using (var scope = fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tracked = await dbContext.Users.SingleAsync(candidate => candidate.Id == user.Id);

            dbContext.Users.Remove(tracked);

            await dbContext.SaveChangesAsync();
        }

        Assert.Null(await FindAsync(user.Id));

        var deleted = await FindAsync(user.Id, includeDeleted: true);

        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
        Assert.Equal(ChangedAt.UtcDateTime, deleted.DeletedAtUtc);
    }

    [Fact]
    public async Task SaveChanges_ShouldFail_WhenClerkUserIdIsAlreadyUsed()
    {
        var user = await AddUserAsync();

        using var scope = fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Users.Add(UserFactory.Create(clerkUserId: user.ClerkUserId));

        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChanges_ShouldDispatchAndClearDomainEvents()
    {
        var user = await AddUserAsync();

        Assert.Empty(user.DomainEvents);
        Assert.Single(fixture.DomainEvents.UserCreated, domainEvent => domainEvent.UserId == user.Id);
    }

    private async Task<User> AddUserAsync()
    {
        fixture.TimeProvider.UtcNow = CreatedAt;

        using var scope = fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = UserFactory.Create(clerkUserId: $"user_{Guid.NewGuid():N}");

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        return user;
    }

    private async Task<User?> FindAsync(Guid id, bool includeDeleted = false)
    {
        using var scope = fixture.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.AsNoTracking();

        if (includeDeleted)
        {
            users = users.IgnoreQueryFilters();
        }

        return await users.SingleOrDefaultAsync(user => user.Id == id);
    }
}
