using MediatR;
using SocialMedia.Domain.Primitives;

namespace SocialMedia.Application.Abstractions;

public sealed record DomainEventNotification<TEvent>(TEvent DomainEvent) : INotification
    where TEvent : IDomainEvent;
