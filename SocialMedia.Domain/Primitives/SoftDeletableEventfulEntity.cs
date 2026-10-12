namespace SocialMedia.Domain.Primitives;

public abstract class SoftDeletableEventfulEntity : AuditableEventfulEntity, ISoftDeletable
{
    protected SoftDeletableEventfulEntity(Guid id)
        : base(id)
    {
    }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }
}
