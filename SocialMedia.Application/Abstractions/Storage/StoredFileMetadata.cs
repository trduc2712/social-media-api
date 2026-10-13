namespace SocialMedia.Application.Abstractions.Storage;

public sealed record StoredFileMetadata(
    string Key,
    long SizeInBytes,
    string ContentType,
    DateTimeOffset LastModifiedAt);
