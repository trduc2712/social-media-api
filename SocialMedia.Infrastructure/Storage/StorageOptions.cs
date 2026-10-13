namespace SocialMedia.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string ServiceUrl { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;

    public string AccessKeyId { get; set; } = string.Empty;

    public string SecretAccessKey { get; set; } = string.Empty;

    public bool ForcePathStyle { get; set; }

    public TimeSpan UploadUrlLifetime { get; set; } = TimeSpan.FromMinutes(15);

    public TimeSpan DownloadUrlLifetime { get; set; } = TimeSpan.FromHours(1);
}
