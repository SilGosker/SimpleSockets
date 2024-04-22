using EasySockets.Services;
using EasySockets.Services.Caching;
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
    /// <param name="configure">A function to configure the provided <see cref="EasySocketGlobalOptions" />.</param>
    public static IServiceCollection AddEasySocketServices(this IServiceCollection serviceCollection,
        Action<EasySocketGlobalOptions>? configure = null)
    {
        if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

        if (serviceCollection.Any(x => x.ServiceType == typeof(IEasySocketService)))
            throw new InvalidOperationException(
                "The EasySocketService has already been added to the service collection.");

        serviceCollection.AddSingleton<EasySocketService>();
        serviceCollection.AddSingleton<IEasySocketService>(e => e.GetRequiredService<EasySocketService>());

        serviceCollection.AddSingleton<EasySocketAuthenticationService>();
        serviceCollection.AddSingleton<EasySocketTypeHolder>();

        serviceCollection.Configure(configure ?? (_ => { }));

        return serviceCollection;
    }
}