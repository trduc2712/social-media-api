namespace SocialMedia.Domain.Primitives;

public abstract class EventfulEntity : BaseEntity, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected EventfulEntity(Guid id)
        : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
