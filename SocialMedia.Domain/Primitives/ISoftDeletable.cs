namespace SocialMedia.Domain.Primitives;

public interface ISoftDeletable
{
    bool IsDeleted { get; }

    DateTime? DeletedAtUtc { get; }
}
