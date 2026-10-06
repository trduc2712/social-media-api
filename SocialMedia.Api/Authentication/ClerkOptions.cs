namespace SocialMedia.Api.Authentication;

public sealed class ClerkOptions
{
    public const string SectionName = "Clerk";

    public string Authority { get; set; } = string.Empty;

    public string[] AuthorizedParties { get; set; } = [];

    public string PublishableKey { get; set; } = string.Empty;
}
