using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Application.Abstractions.Storage;
using SocialMedia.Infrastructure.Storage;
using Testcontainers.Minio;

namespace SocialMedia.Infrastructure.IntegrationTests.Storage;

public sealed class StorageFixture : IAsyncLifetime
{
    private const string Image = "pgsty/minio:RELEASE.2026-08-04T00-00-00Z";
    private const string Region = "us-east-1";
    private const string BucketName = "social-media-tests";

    private readonly MinioContainer _container = new MinioBuilder(Image).Build();

    private ServiceProvider _serviceProvider = null!;

    public static TimeSpan UploadUrlLifetime { get; } = TimeSpan.FromMinutes(5);

    public static TimeSpan DownloadUrlLifetime { get; } = TimeSpan.FromMinutes(10);

    public HttpClient HttpClient { get; } = new();

    public IStorageService StorageService => _serviceProvider.GetRequiredService<IStorageService>();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var services = new ServiceCollection();

        services.AddStorage(options =>
        {
            options.ServiceUrl = _container.GetConnectionString();
            options.Region = Region;
            options.BucketName = BucketName;
            options.AccessKeyId = _container.GetAccessKey();
            options.SecretAccessKey = _container.GetSecretKey();
            options.ForcePathStyle = true;
            options.UploadUrlLifetime = UploadUrlLifetime;
            options.DownloadUrlLifetime = DownloadUrlLifetime;
        });

        _serviceProvider = services.BuildServiceProvider();

        await _serviceProvider.GetRequiredService<IAmazonS3>().PutBucketAsync(BucketName);
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();

        await _serviceProvider.DisposeAsync();
        await _container.DisposeAsync();
    }
}
