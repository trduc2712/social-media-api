using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Abstractions.Storage;

namespace SocialMedia.Infrastructure.Storage;

internal sealed class S3StorageService(
    IAmazonS3 client,
    IOptions<StorageOptions> options,
    TimeProvider timeProvider) : IStorageService
{
    private readonly StorageOptions _options = options.Value;

    public async Task<PresignedUpload> CreateUploadUrlAsync(
        string key,
        string contentType,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var expiresAt = timeProvider.GetUtcNow().Add(_options.UploadUrlLifetime);
        var request = CreatePresignedUrlRequest(key, HttpVerb.PUT, expiresAt);
        request.ContentType = contentType;

        var url = await client.GetPreSignedURLAsync(request);

        return new PresignedUpload(new Uri(url), contentType, expiresAt);
    }

    public async Task<PresignedDownload> CreateDownloadUrlAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var expiresAt = timeProvider.GetUtcNow().Add(_options.DownloadUrlLifetime);
        var url = await client.GetPreSignedURLAsync(CreatePresignedUrlRequest(key, HttpVerb.GET, expiresAt));

        return new PresignedDownload(new Uri(url), expiresAt);
    }

    public async Task<StoredFileMetadata?> GetMetadataAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetObjectMetadataAsync(_options.BucketName, key, cancellationToken);

            return new StoredFileMetadata(
                key,
                response.ContentLength,
                response.Headers.ContentType,
                new DateTimeOffset(response.LastModified.GetValueOrDefault().ToUniversalTime()));
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public Task DeleteAsync(string key, CancellationToken cancellationToken) =>
        client.DeleteObjectAsync(_options.BucketName, key, cancellationToken);

    private GetPreSignedUrlRequest CreatePresignedUrlRequest(string key, HttpVerb verb, DateTimeOffset expiresAt) =>
        new()
        {
            BucketName = _options.BucketName,
            Key = key,
            Verb = verb,
            Expires = expiresAt.UtcDateTime,
            Protocol = new Uri(_options.ServiceUrl).Scheme == Uri.UriSchemeHttp ? Protocol.HTTP : Protocol.HTTPS,
        };
}
