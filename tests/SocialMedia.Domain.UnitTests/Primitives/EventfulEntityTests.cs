using SocialMedia.Domain.Primitives;

namespace SocialMedia.Domain.UnitTests.Primitives;

public class EventfulEntityTests
{
    [Fact]
    public void RaiseDomainEvent_ShouldAddEvent()
    {
        var entity = new SampleEntity();
        var domainEvent = new SampleEvent();

        entity.Raise(domainEvent);

        Assert.Same(domainEvent, Assert.Single(entity.DomainEvents));
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var entity = new SampleEntity();
        entity.Raise(new SampleEvent());
        entity.Raise(new SampleEvent());

        entity.ClearDomainEvents();

        Assert.Empty(entity.DomainEvents);
    }

    [Fact]
    public void DomainEvent_ShouldRecordWhenItOccurred()
    {
        var before = DateTime.UtcNow;

        var domainEvent = new SampleEvent();

        Assert.InRange(domainEvent.OccurredOnUtc, before, DateTime.UtcNow);
        Assert.Equal(DateTimeKind.Utc, domainEvent.OccurredOnUtc.Kind);
    }

    private sealed class SampleEntity() : EventfulEntity(Guid.NewGuid())
    {
        public void Raise(IDomainEvent domainEvent) => RaiseDomainEvent(domainEvent);
    }

    private sealed record SampleEvent : DomainEvent;
}
