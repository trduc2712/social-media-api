using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SocialMedia.Infrastructure.Interceptors;

namespace SocialMedia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<DomainEventsInterceptor>();
        services.AddSingleton<AuditingInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) => options
            .UseNpgsql(connectionString)
            .AddInterceptors(
                serviceProvider.GetRequiredService<DomainEventsInterceptor>(),
                serviceProvider.GetRequiredService<AuditingInterceptor>()));

        return services;
    }
}
