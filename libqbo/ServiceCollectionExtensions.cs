using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace libqbo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQboClient(this IServiceCollection services)
    {
        services.AddScoped<QboAuthenticationHandler>();
        services.AddScoped<QboClient>();
        return services;
    }

    public static IServiceCollection ConfigureQbo(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.Configure<Configuration.QboConfiguration>(configuration);
        return services;
    }
}
