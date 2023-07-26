using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.Services;

namespace SimpleSockets.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="ISimpleSocketService"/> to the dependency container
    /// </summary>
    /// <param name="serviceCollection">The collection the <see cref="ISimpleSocketService"/> should be added to </param>
    public static void AddSimpleSocketService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ISimpleSocketService, SimpleSocketService>();
    }
}