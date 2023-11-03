using Microsoft.Extensions.DependencyInjection;
using EasySockets.Services;

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
    public static void AddEasySocketServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<EasySocketService>(_ => EasySocketService.Create());
        serviceCollection.AddSingleton<IEasySocketService>(e => e.GetRequiredService<EasySocketService>());
    }
}