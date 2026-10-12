namespace SocialMedia.Domain.Primitives;

public abstract class BaseEntity : IEquatable<BaseEntity>
{
    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    public static bool operator ==(BaseEntity? left, BaseEntity? right) => Equals(left, right);

    public static bool operator !=(BaseEntity? left, BaseEntity? right) => !Equals(left, right);

    public bool Equals(BaseEntity? other) =>
        other is not null && other.GetType() == GetType() && other.Id == Id;

    public override bool Equals(object? obj) => obj is BaseEntity other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
