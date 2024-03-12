using EasySockets.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EasySockets.Mock;

public class MockEasySocketAuthenticatorWithDependency : IEasySocketAuthenticator
{
    internal const bool Authenticated = true;
    internal const string RoomId = "MockEasySocketAuthenticatorWithDependencyRoomId";
    internal const string ClientId = "MockEasySocketAuthenticatorWithDependencyClientId";
    public MockEasySocketAuthenticatorWithDependency(IServiceScopeFactory _)
    {
    }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        return new EasySocketAuthenticationResult(Authenticated, RoomId, ClientId);
    }
}