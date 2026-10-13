namespace SocialMedia.Application.Abstractions.Storage;

public sealed record PresignedUpload(Uri Url, string ContentType, DateTimeOffset ExpiresAt);
