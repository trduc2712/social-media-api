using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SocialMedia.Infrastructure.IntegrationTests.Storage;

public class S3StorageServiceTests(StorageFixture fixture) : IClassFixture<StorageFixture>
{
    private const string ContentType = "text/plain";

    private static readonly byte[] Content = Encoding.UTF8.GetBytes("hello storage");

    [Fact]
    public async Task CreateUploadUrl_ShouldAllowUploadingTheFile()
    {
        var key = NewKey();

        var response = await UploadAsync(key, ContentType);
        var metadata = await fixture.StorageService.GetMetadataAsync(key, CancellationToken.None);

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(metadata);
        Assert.Equal(key, metadata.Key);
        Assert.Equal(Content.Length, metadata.SizeInBytes);
        Assert.Equal(ContentType, metadata.ContentType);
    }

    [Fact]
    public async Task CreateUploadUrl_ShouldRejectUpload_WhenContentTypeDiffersFromTheSignedOne()
    {
        var key = NewKey();
        var upload = await fixture.StorageService.CreateUploadUrlAsync(key, ContentType, CancellationToken.None);

        var response = await PutAsync(upload.Url, "image/png");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(await fixture.StorageService.GetMetadataAsync(key, CancellationToken.None));
    }

    [Fact]
    public async Task CreateUploadUrl_ShouldExpireAfterTheConfiguredLifetime()
    {
        var before = DateTimeOffset.UtcNow;

        var upload = await fixture.StorageService.CreateUploadUrlAsync(NewKey(), ContentType, CancellationToken.None);

        var after = DateTimeOffset.UtcNow;

        Assert.Equal(ContentType, upload.ContentType);
        Assert.InRange(
            upload.ExpiresAt,
            before.Add(StorageFixture.UploadUrlLifetime),
            after.Add(StorageFixture.UploadUrlLifetime));
    }

    [Fact]
    public async Task CreateDownloadUrl_ShouldAllowDownloadingTheFile()
    {
        var key = NewKey();
        await UploadAsync(key, ContentType);

        var download = await fixture.StorageService.CreateDownloadUrlAsync(key, CancellationToken.None);
        var response = await fixture.HttpClient.GetAsync(download.Url);

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(Content, await response.Content.ReadAsByteArrayAsync());
        Assert.Equal(ContentType, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task CreateDownloadUrl_ShouldExpireAfterTheConfiguredLifetime()
    {
        var before = DateTimeOffset.UtcNow;

        var download = await fixture.StorageService.CreateDownloadUrlAsync(NewKey(), CancellationToken.None);

        var after = DateTimeOffset.UtcNow;

        Assert.InRange(
            download.ExpiresAt,
            before.Add(StorageFixture.DownloadUrlLifetime),
            after.Add(StorageFixture.DownloadUrlLifetime));
    }

    [Fact]
    public async Task GetMetadata_ShouldReturnNull_WhenFileDoesNotExist()
    {
        var metadata = await fixture.StorageService.GetMetadataAsync(NewKey(), CancellationToken.None);

        Assert.Null(metadata);
    }

    [Fact]
    public async Task Delete_ShouldRemoveTheFile()
    {
        var key = NewKey();
        await UploadAsync(key, ContentType);

        await fixture.StorageService.DeleteAsync(key, CancellationToken.None);

        Assert.Null(await fixture.StorageService.GetMetadataAsync(key, CancellationToken.None));
    }

    [Fact]
    public async Task Delete_ShouldNotFail_WhenFileDoesNotExist()
    {
        var exception = await Record.ExceptionAsync(
            () => fixture.StorageService.DeleteAsync(NewKey(), CancellationToken.None));

        Assert.Null(exception);
    }

    private static string NewKey() => $"tests/{Guid.NewGuid():N}.txt";

    private async Task<HttpResponseMessage> UploadAsync(string key, string contentType)
    {
        var upload = await fixture.StorageService.CreateUploadUrlAsync(key, contentType, CancellationToken.None);

        return await PutAsync(upload.Url, contentType);
    }

    private async Task<HttpResponseMessage> PutAsync(Uri url, string contentType)
    {
        using var content = new ByteArrayContent(Content);
        content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        return await fixture.HttpClient.PutAsync(url, content);
    }
}
