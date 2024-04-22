using EasySockets.Authentication;
using EasySockets.Builder;
using EasySockets.Mock;
using EasySockets.Services.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace EasySockets.Services;

public class EasySocketAuthenticationServiceTests
{
    [Fact]
    public async Task GetAuthenticationResultAsync_WhenNoAuthenticatorsAreConfigured_ShouldReturnTrue()
    {
        // Arrange
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var optionsMock = new Mock<IOptions<EasySocketGlobalOptions>>();
        optionsMock.SetupGet(x => x.Value).Returns(new EasySocketGlobalOptions());

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new());

        var context = new DefaultHttpContext();

        var authenticator = new EasySocketAuthenticationService(serviceScopeFactoryMock.Object, optionsMock.Object);

        // Act
        var task = authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);
        var authenticationResult = await task;

        // Assert
        Assert.NotNull(task);
        Assert.True(authenticationResult.IsAuthenticated);
    }

    [Theory]
    [InlineData(true, "roomId", "clientId")]
    [InlineData(false, "roomId", "clientId")]
    public async Task GetAuthenticationResultAsync_WhenAuthenticatorsAreConfigured_UsesAuthenticatorForAuthenticationResult(bool authenticated, string roomId, string clientId)
    {
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var optionsMock = new Mock<IOptions<EasySocketGlobalOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EasySocketGlobalOptions()
        {
            GetDefaultClientId = _ => clientId,
            GetDefaultRoomId = _ => roomId
        });

        MockEasySocketAuthenticator.Authenticated = authenticated;
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

        var authenticator = new EasySocketAuthenticationService(context.RequestServices.GetRequiredService<IServiceScopeFactory>(), optionsMock.Object);

        // Act
        var authenticationResult = await authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);

        // Assert
        Assert.Equal(authenticated, authenticationResult.IsAuthenticated);
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
        var mockType = new Mock<IEasySocketAuthenticator>();
        mockType.Setup(x => x.Authenticate(It.IsAny<EasySocketAuthenticationResult>(), It.IsAny<HttpContext>()))
            .Returns(true);

        var optionsMock = new Mock<IOptions<EasySocketGlobalOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EasySocketGlobalOptions()
        {
            GetDefaultClientId = _ => clientId!,
            GetDefaultRoomId = _ => roomId!
        });

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new()
        {
            Authenticators = new List<Type> { mockType.Object.GetType() }
        });

        var context = new DefaultHttpContext()
        {
            RequestServices = serviceProvider
        };

        var authenticator = new EasySocketAuthenticationService(context.RequestServices.GetRequiredService<IServiceScopeFactory>(), optionsMock.Object);

        var authenticationResult = authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await authenticationResult);
    }

    [Fact]
    public async Task GetAuthenticationResultAsync_WhenAuthenticatorsHaveDependencies_InjectsDependencies()
    {
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var optionsMock = new Mock<IOptions<EasySocketGlobalOptions>>();
        optionsMock.Setup(x => x.Value).Returns(new EasySocketGlobalOptions());

        var easySocketTypeCache = new EasySocketTypeCache(typeof(EasySocket), new()
        {
            Authenticators = new List<Type> { typeof(MockEasySocketAuthenticatorWithDependency) }
        });

        var context = new DefaultHttpContext()
        {
            RequestServices = serviceProvider
        };
        var authenticator = new EasySocketAuthenticationService(context.RequestServices.GetRequiredService<IServiceScopeFactory>(), optionsMock.Object);

        var authenticationResult = await authenticator.GetAuthenticationResultAsync(easySocketTypeCache, context);

        Assert.Equal(MockEasySocketAuthenticatorWithDependency.Authenticated, authenticationResult.IsAuthenticated);
        Assert.Equal(MockEasySocketAuthenticatorWithDependency.RoomId, authenticationResult.RoomId);
        Assert.Equal(MockEasySocketAuthenticatorWithDependency.ClientId, authenticationResult.ClientId);
    }
}