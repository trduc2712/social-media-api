using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using SocialMedia.Api.Authentication;

namespace SocialMedia.Api.OpenApi;

public static class OpenApiExtensions
{
    public static IServiceCollection AddApiDocumentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ApiDocumentationOptions>()
            .Bind(configuration.GetSection(ApiDocumentationOptions.SectionName));

        services
            .AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApi(options =>
            {
                options.Document.AddDocumentTransformer((document, context, _) =>
                {
                    var documentation = context.ApplicationServices
                        .GetRequiredService<IOptions<ApiDocumentationOptions>>().Value;

                    document.Info.Title = documentation.Title;
                    document.Info.Description = documentation.Description;

                    return Task.CompletedTask;
                });
                options.Document.AddDocumentTransformer<BearerSecurityTransformer>();
                options.Document.AddOperationTransformer<BearerSecurityTransformer>();
            });

        return services;
    }

    public static WebApplication UseApiDocumentation(this WebApplication app)
    {
        var documentation = app.Services.GetRequiredService<IOptions<ApiDocumentationOptions>>().Value;

        if (!documentation.Enabled)
        {
            return app;
        }

        app.MapOpenApi().WithDocumentPerVersion().AllowAnonymous();

        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = documentation.Title;

            foreach (var version in app.Services.GetRequiredService<IApiVersionDescriptionProvider>()
                .ApiVersionDescriptions
                .OrderByDescending(description => description.ApiVersion))
            {
                options.SwaggerEndpoint($"/openapi/{version.GroupName}.json", version.GroupName);
            }

            options.EnablePersistAuthorization();
            options.EnableDeepLinking();
            options.EnableTryItOutByDefault();
            options.DisplayRequestDuration();
            options.UseClerkLogin(app.Services.GetRequiredService<IOptions<ClerkOptions>>().Value);
        });

        return app;
    }
}
