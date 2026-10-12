using SocialMedia.Domain.Primitives;

namespace SocialMedia.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : DomainEvent;
