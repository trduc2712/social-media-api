using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SocialMedia.Application.Abstractions;
using SocialMedia.Domain.Primitives;

namespace SocialMedia.Infrastructure.Interceptors;

public sealed class DomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            await DispatchAsync(eventData.Context, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchAsync(DbContext context, CancellationToken cancellationToken)
    {
        while (true)
        {
            var entities = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count > 0)
                .ToList();

            if (entities.Count == 0)
            {
                return;
            }

            var domainEvents = entities.SelectMany(entity => entity.DomainEvents).ToList();

            entities.ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = Activator.CreateInstance(notificationType, domainEvent)!;

                await publisher.Publish(notification, cancellationToken);
            }
        }
    }
}
