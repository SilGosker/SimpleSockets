using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.Services;

namespace SimpleSockets.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSimpleSocketService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<SimpleSocketService>();
    }
}