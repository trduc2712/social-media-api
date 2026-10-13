using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialMedia.Infrastructure.Storage;

namespace SocialMedia.Infrastructure.IntegrationTests.Storage;

public class StorageOptionsValidationTests
{
    public static TheoryData<string> RequiredSettings =>
    [
        nameof(StorageOptions.ServiceUrl),
        nameof(StorageOptions.Region),
        nameof(StorageOptions.BucketName),
        nameof(StorageOptions.AccessKeyId),
        nameof(StorageOptions.SecretAccessKey),
    ];

    [Fact]
    public void Options_ShouldBeValid_WhenEverySettingIsProvided()
    {
        var options = Resolve(_ => { });

        Assert.Equal("social-media", options.BucketName);
    }

    [Theory]
    [MemberData(nameof(RequiredSettings))]
    public void Options_ShouldBeInvalid_WhenRequiredSettingIsMissing(string setting)
    {
        var exception = Assert.Throws<OptionsValidationException>(() =>
            Resolve(options => typeof(StorageOptions).GetProperty(setting)!.SetValue(options, string.Empty)));

        Assert.Contains(exception.Failures, failure => failure.Contains($"{StorageOptions.SectionName}:{setting}"));
    }

    private static StorageOptions Resolve(Action<StorageOptions> change)
    {
        var services = new ServiceCollection();

        services.AddStorage(options =>
        {
            options.ServiceUrl = "https://s3.eu-central-003.backblazeb2.com";
            options.Region = "eu-central-003";
            options.BucketName = "social-media";
            options.AccessKeyId = "access-key-id";
            options.SecretAccessKey = "secret-access-key";

            change(options);
        });

        using var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<IOptions<StorageOptions>>().Value;
    }
}
