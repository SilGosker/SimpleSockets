using EasySockets.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasySockets.Builder;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEasySocketServices_ExposesRequiredServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasySocketServices();

        Assert.Contains(serviceCollection, x => x.Lifetime == ServiceLifetime.Singleton && x.ServiceType == typeof(IEasySocketService));
        Assert.Contains(serviceCollection, x => x.Lifetime == ServiceLifetime.Singleton && x.ServiceType == typeof(EasySocketTypeHolder));
        Assert.Contains(serviceCollection, x => x.Lifetime == ServiceLifetime.Singleton && x.ServiceType == typeof(EasySocketAuthenticator));
    }

    [Fact]
    public void AddEasySocketServices_ShouldExposeEasySocketMiddlewareOptions()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasySocketServices();

        var services = serviceCollection.BuildServiceProvider();
        services.GetRequiredService<IOptions<EasySocketMiddlewareOptions>>();
    }

    [Fact]
    public void AddEasySocketServices_ConfiguresEasySocketMiddleWareOptions()
    {
        var keepAliveInterval = TimeSpan.FromSeconds(1234);
        var receiveBufferSize = 1234;

        var websocketOptions = new WebSocketOptions
        {
            KeepAliveInterval = keepAliveInterval,
            ReceiveBufferSize = receiveBufferSize,
        };

        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddEasySocketServices(options =>
        {
            options.GetDefaultClientId = _ => "Test";
            options.GetDefaultRoomId = _ => "Test";
            options.IsDefaultAuthenticated = true;
            options.WebSocketOptions = websocketOptions;
        });

        var services = serviceCollection.BuildServiceProvider();
        var options = services.GetRequiredService<IOptions<EasySocketMiddlewareOptions>>();


        Assert.Equal("Test", options.Value.GetDefaultClientId(null!));
        Assert.Equal("Test", options.Value.GetDefaultRoomId(null!));
        Assert.True(options.Value.IsDefaultAuthenticated);
        Assert.Equal(keepAliveInterval, options.Value.WebSocketOptions.KeepAliveInterval);
        Assert.Equal(receiveBufferSize, options.Value.WebSocketOptions.ReceiveBufferSize);
    }

    [Fact]
    public void AddEasySocketServices_WhenEasySocketServiceIsAlreadyAdded_ShouldThrowInvalidOperationException()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasySocketServices();

        Assert.Throws<InvalidOperationException>(() => serviceCollection.AddEasySocketServices());
    }

    [Fact]
    public void AddEasySocketServices_WhenServiceCollectionIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddEasySocketServices(null!));
    }
}