namespace SocialMedia.Domain.Primitives;

public abstract class AuditableEventfulEntity : EventfulEntity, IAuditable
{
    protected AuditableEventfulEntity(Guid id)
        : base(id)
    {
    }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }
}
