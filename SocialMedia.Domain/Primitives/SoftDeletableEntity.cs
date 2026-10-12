namespace SocialMedia.Domain.Primitives;

public abstract class SoftDeletableEntity : AuditableEntity, ISoftDeletable
{
    protected SoftDeletableEntity(Guid id)
        : base(id)
    {
    }

    public bool IsDeleted { get; private set; }

    public DateTime? DeletedAtUtc { get; private set; }
}
