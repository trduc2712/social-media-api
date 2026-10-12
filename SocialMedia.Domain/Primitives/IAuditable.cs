namespace SocialMedia.Domain.Primitives;

public interface IAuditable
{
    DateTime CreatedAtUtc { get; }

    DateTime? UpdatedAtUtc { get; }
}
