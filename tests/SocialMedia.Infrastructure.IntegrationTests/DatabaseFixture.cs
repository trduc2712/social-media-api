using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SocialMedia.Application.Abstractions;
using SocialMedia.Domain.Users.Events;

namespace SocialMedia.Infrastructure.IntegrationTests;

public sealed class DatabaseFixture : IAsyncLifetime
{
    public const string ConnectionStringVariable = "TEST_DATABASE_CONNECTION_STRING";

    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=2712";

    private ServiceProvider _serviceProvider = null!;

    public FixedTimeProvider TimeProvider { get; } = new();

    public RecordedDomainEvents DomainEvents { get; } = new();

    public IServiceScope CreateScope() => _serviceProvider.CreateScope();

    public async Task InitializeAsync()
    {
        var connectionString = new NpgsqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable(ConnectionStringVariable) ?? DefaultConnectionString)
        {
            Database = $"social-media-tests-{Guid.NewGuid():N}",
        };

        var services = new ServiceCollection();

        services.AddSingleton<TimeProvider>(TimeProvider);
        services.AddSingleton(DomainEvents);
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DatabaseFixture).Assembly));
        services.AddInfrastructure(connectionString.ConnectionString);

        _serviceProvider = services.BuildServiceProvider();

        using var scope = CreateScope();

        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        using (var scope = CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureDeletedAsync();
        }

        await _serviceProvider.DisposeAsync();
    }

    public sealed class FixedTimeProvider : TimeProvider
    {
        public DateTimeOffset UtcNow { get; set; } = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow() => UtcNow;
    }

    public sealed class RecordedDomainEvents
    {
        public List<UserCreatedDomainEvent> UserCreated { get; } = [];
    }

    public sealed class UserCreatedRecorder(RecordedDomainEvents recorded)
        : INotificationHandler<DomainEventNotification<UserCreatedDomainEvent>>
    {
        public Task Handle(
            DomainEventNotification<UserCreatedDomainEvent> notification,
            CancellationToken cancellationToken)
        {
            recorded.UserCreated.Add(notification.DomainEvent);

            return Task.CompletedTask;
        }
    }
}
