namespace SocialMedia.Application.Abstractions.Storage;

public sealed record PresignedDownload(Uri Url, DateTimeOffset ExpiresAt);
