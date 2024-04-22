using EasySockets.Services;
using EasySockets.Services.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasySockets.Builder;

public class ServiceCollectionExtensionsTests
{
    [Theory]
    [InlineData(typeof(IEasySocketService))]
    [InlineData(typeof(EasySocketTypeHolder))]
    [InlineData(typeof(EasySocketAuthenticationService))]
    [InlineData(typeof(EasySocketService))]
    public void AddEasySocketServices_ExposesRequiredServices(Type type)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasySocketServices();

        Assert.Contains(serviceCollection, x => x.Lifetime == ServiceLifetime.Singleton && x.ServiceType == type);
    }

    [Fact]
    public void AddEasySocketServices_ShouldExposeEasySocketGlobalOptions()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddEasySocketServices();

        var services = serviceCollection.BuildServiceProvider();
        var options = services.GetRequiredService<IOptions<EasySocketGlobalOptions>>();

        Assert.NotNull(options);
        Assert.NotNull(options.Value);
    }

    [Fact]
    public void AddEasySocketServices_WhenConfiguring_ConfiguresEasySocketGlobalOptions()
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
            options.WebSocketOptions = websocketOptions;
        });

        var services = serviceCollection.BuildServiceProvider();
        var options = services.GetRequiredService<IOptions<EasySocketGlobalOptions>>();


        Assert.Equal("Test", options.Value.GetDefaultClientId(null!));
        Assert.Equal("Test", options.Value.GetDefaultRoomId(null!));
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