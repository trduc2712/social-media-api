using System.Net;
using SocialMedia.Api.Authentication;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SocialMedia.Api.OpenApi;

public static class SwaggerClerkLogin
{
    private const string ClerkJsVersion = "5";
    private const string ScriptResource = "SocialMedia.Api.OpenApi.swagger-clerk.js";
    private const string PluginName = "ClerkAuthPlugin";

    public static void UseClerkLogin(this SwaggerUIOptions options, ClerkOptions clerk)
    {
        if (string.IsNullOrWhiteSpace(clerk.PublishableKey))
        {
            return;
        }

        var publishableKey = WebUtility.HtmlEncode(clerk.PublishableKey);
        var clerkScriptUrl = WebUtility.HtmlEncode(
            $"{clerk.Authority.TrimEnd('/')}/npm/@clerk/clerk-js@{ClerkJsVersion}/dist/clerk.browser.js");

        options.HeadContent += $"""
            <script async crossorigin="anonymous" data-clerk-publishable-key="{publishableKey}" src="{clerkScriptUrl}"></script>
            <script>{ReadResource(ScriptResource)}</script>
            """;
        options.UseRequestInterceptor("(request) => { return window.attachClerkToken(request); }");
        options.ConfigObject.Plugins = [.. options.ConfigObject.Plugins ?? [], PluginName];
    }

    private static string ReadResource(string name)
    {
        using var stream = typeof(SwaggerClerkLogin).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"Embedded resource '{name}' was not found.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
