namespace SocialMedia.Domain.Primitives;

public abstract class AuditableEntity : BaseEntity, IAuditable
{
    protected AuditableEntity(Guid id)
        : base(id)
    {
    }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }
}
