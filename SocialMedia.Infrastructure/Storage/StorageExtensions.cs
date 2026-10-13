using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Abstractions.Storage;

namespace SocialMedia.Infrastructure.Storage;

public static class StorageExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services, Action<StorageOptions> configure)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddOptions<StorageOptions>()
            .Configure(configure)
            .Validate(
                options => Uri.TryCreate(options.ServiceUrl, UriKind.Absolute, out _),
                $"{StorageOptions.SectionName}:{nameof(StorageOptions.ServiceUrl)} must be an absolute URL.")
            .Require(options => options.Region, nameof(StorageOptions.Region))
            .Require(options => options.BucketName, nameof(StorageOptions.BucketName))
            .Require(options => options.AccessKeyId, nameof(StorageOptions.AccessKeyId))
            .Require(options => options.SecretAccessKey, nameof(StorageOptions.SecretAccessKey))
            .ValidateOnStart();

        services.AddSingleton<IAmazonS3>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<StorageOptions>>().Value;

            return new AmazonS3Client(
                new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey),
                new AmazonS3Config
                {
                    ServiceURL = options.ServiceUrl,
                    AuthenticationRegion = options.Region,
                    ForcePathStyle = options.ForcePathStyle,
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                    ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
                });
        });

        services.AddSingleton<IStorageService, S3StorageService>();

        return services;
    }

    private static OptionsBuilder<StorageOptions> Require(
        this OptionsBuilder<StorageOptions> builder,
        Func<StorageOptions, string> setting,
        string name) =>
        builder.Validate(
            options => !string.IsNullOrWhiteSpace(setting(options)),
            $"{StorageOptions.SectionName}:{name} is required.");
}
