namespace SocialMedia.Application.Abstractions.Storage;

public interface IStorageService
{
    Task<PresignedUpload> CreateUploadUrlAsync(string key, string contentType, CancellationToken cancellationToken);

    Task<PresignedDownload> CreateDownloadUrlAsync(string key, CancellationToken cancellationToken);

    Task<StoredFileMetadata?> GetMetadataAsync(string key, CancellationToken cancellationToken);

    Task DeleteAsync(string key, CancellationToken cancellationToken);
}
