using EasySockets.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasySockets.Builder;

/// <summary>
///     The exposed EasySockets extensions for the <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the <see cref="IEasySocketService" /> to the dependency injection container.
    /// </summary>
    /// <param name="serviceCollection">The collection the <see cref="IEasySocketService" /> should be added to.</param>
    /// <param name="configure">A function to configure the provided <see cref="EasySocketMiddlewareOptions" />.</param>
    public static void AddEasySocketServices(this IServiceCollection serviceCollection,
        Action<EasySocketMiddlewareOptions>? configure = null)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

        if (serviceCollection.Any(x => x.ServiceType == typeof(IEasySocketService)))
            throw new InvalidOperationException("The EasySocketService has already been added to the service collection.");

        serviceCollection.AddSingleton<EasySocketService>();
        serviceCollection.AddSingleton<IEasySocketService>(e => e.GetRequiredService<EasySocketService>());

        serviceCollection.AddSingleton<EasySocketAuthenticator>();
        serviceCollection.AddSingleton<EasySocketTypeHolder>();

        if (configure != null)
        {
            serviceCollection.Configure<EasySocketMiddlewareOptions>(configure);
        }
        else
        {
            serviceCollection.AddOptions<EasySocketMiddlewareOptions>();
        }
    }
}