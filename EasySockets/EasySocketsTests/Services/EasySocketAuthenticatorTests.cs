using EasySockets.Authentication;
using EasySockets.Builder;
using EasySockets.DataModels;
using EasySockets.Mock;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace EasySockets.Services;

public class EasySocketAuthenticatorTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAuthenticationResultAsync_WhenNoAuthenticatorsAreConfigured_ShouldReturnConfiguredAuthenticationResult(bool globallyAuthenticated)
    {
        // Arrange
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var optionsMock = new Mock<IOptions<EasySocketMiddlewareOptions>>();
        optionsMock.SetupGet(x => x.Value).Returns(new EasySocketMiddlewareOptions()
        {
            IsDefaultAuthenticated = globallyAuthenticated
        });

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new());

        var context = new DefaultHttpContext();

        var authenticator = new EasySocketAuthenticator(serviceScopeFactoryMock.Object, optionsMock.Object);

        // Act
        var task = authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);
        var authenticationResult = await task;

        // Assert
        Assert.NotNull(task);
        Assert.Equal(globallyAuthenticated, authenticationResult.IsAuthenticated);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task GetAuthenticationResultAsync_WhenNoAuthenticatorsAreConfigured_ShouldPreferSocketSpecificAuthenticationResult(
            bool globallyAuthenticated, bool socketAuthenticated)
    {
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var optionsMock = new Mock<IOptions<EasySocketMiddlewareOptions>>();
        optionsMock.SetupGet(x => x.Value).Returns(new EasySocketMiddlewareOptions()
        {
            IsDefaultAuthenticated = globallyAuthenticated
        });

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new()
        {
            IsDefaultAuthenticated = socketAuthenticated
        });

        var context = new DefaultHttpContext();

        var authenticator = new EasySocketAuthenticator(serviceScopeFactoryMock.Object, optionsMock.Object);

        var task = authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);
        var authenticationResult = await task;

        Assert.NotNull(task);
        Assert.Equal(socketAuthenticated, authenticationResult.IsAuthenticated);
    }

    [Theory]
    [InlineData(true, "roomId", "clientId")]
    [InlineData(false, "roomId", "clientId")]
    public async Task GetAuthenticationResultAsync_WhenAuthenticatorsAreConfigured_UsesAuthenticatorForAuthenticationResult(bool authenticated, string roomId, string clientId)
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var optionsMock = new Mock<IOptions<EasySocketMiddlewareOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EasySocketMiddlewareOptions()
        {
            IsDefaultAuthenticated = authenticated,
            GetDefaultClientId = _ => clientId,
            GetDefaultRoomId = _ => roomId
        });

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new EasySocketOptions
        {
            Authenticators = new List<Type>
            {
                typeof(MockEasySocketAuthenticator)
            }
        });

        var context = new DefaultHttpContext
        {
            RequestServices = serviceProvider
        };

        var authenticator = new EasySocketAuthenticator(context.RequestServices.GetRequiredService<IServiceScopeFactory>(), optionsMock.Object);

        // Act
        var authenticationResult = await authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);

        // Assert
        Assert.Equal(MockEasySocketAuthenticator.Authenticated, authenticationResult.IsAuthenticated);
        Assert.Equal(MockEasySocketAuthenticator.RoomId, authenticationResult.RoomId);
        Assert.Equal(MockEasySocketAuthenticator.ClientId, authenticationResult.ClientId);
    }

    [Theory]
    [InlineData( null, "customClientId")]
    [InlineData( "customRoomId", null)]
    public async Task GetAuthenticationResultAsync_WhenGetDefaultClientIdOrGetDefaultRoomIdReturnNullAfterSuccessfulAuthentication_ShouldThrowInvalidOperationException(
            string? roomId, string? clientId)
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var optionsMock = new Mock<IOptions<EasySocketMiddlewareOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EasySocketMiddlewareOptions()
        {
            GetDefaultClientId = _ => clientId!,
            GetDefaultRoomId = _ => roomId!
        });

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new()
        {
            Authenticators = new List<Type> { typeof(MockEasySocketAuthenticatorReturningNull) }
        });

        var context = new DefaultHttpContext()
        {
            RequestServices = serviceProvider
        };

        var authenticator = new EasySocketAuthenticator(context.RequestServices.GetRequiredService<IServiceScopeFactory>(), optionsMock.Object);

        var authenticationResult = authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await authenticationResult);
    }

    [Fact]
    public async Task GetAuthenticationResultAsync_WhenAuthenticatorsHaveDependencies_InjectsDependencies()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var optionsMock = new Mock<IOptions<EasySocketMiddlewareOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EasySocketMiddlewareOptions());

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new()
        {
            Authenticators = new List<Type> { typeof(MockEasySocketAuthenticatorWithDependency) }
        });

        var context = new DefaultHttpContext()
        {
            RequestServices = serviceProvider
        };
        var authenticator = new EasySocketAuthenticator(context.RequestServices.GetRequiredService<IServiceScopeFactory>(), optionsMock.Object);

        var authenticationResult = await authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);

        Assert.Equal(MockEasySocketAuthenticatorWithDependency.Authenticated, authenticationResult.IsAuthenticated);
        Assert.Equal(MockEasySocketAuthenticatorWithDependency.RoomId, authenticationResult.RoomId);
        Assert.Equal(MockEasySocketAuthenticatorWithDependency.ClientId, authenticationResult.ClientId);
    }
}