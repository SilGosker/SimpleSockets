using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.Services;

namespace SimpleSockets.Builder;

/// <summary>
///     The exposed SimpleSockets extensions for the <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the <see cref="ISimpleSocketService" /> to the dependency injection container.
    /// </summary>
    /// <param name="serviceCollection">The collection the <see cref="ISimpleSocketService" /> should be added to.</param>
    public static void AddSimpleSocketService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SimpleSocketService>(_ => SimpleSocketService.Create());
        serviceCollection.AddSingleton<ISimpleSocketService>(e => e.GetRequiredService<SimpleSocketService>());
    }
}