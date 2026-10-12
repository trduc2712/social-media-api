namespace SocialMedia.Domain.Primitives;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
