namespace SocialMedia.Api.OpenApi;

public sealed class ApiDocumentationOptions
{
    public const string SectionName = "ApiDocumentation";

    public bool Enabled { get; set; }

    public string Title { get; set; } = "Social Media API";

    public string Description { get; set; } = string.Empty;
}
