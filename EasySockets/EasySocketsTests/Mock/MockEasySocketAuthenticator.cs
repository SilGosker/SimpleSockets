using EasySockets.Authentication;
using Microsoft.AspNetCore.Http;

namespace EasySockets.Mock;

public class MockEasySocketAuthenticator : IEasySocketAuthenticator
{
    internal const string RoomId = "MockEasySocketAuthenticator_TestRoomId";
    internal const string ClientId = "MockEasySocketAuthenticator_TestClientId";
    internal static bool Authenticated = true;

    public MockEasySocketAuthenticator()
    {
        ConstructorCallCount++;
    }

    internal static int ConstructorCallCount { get; set; }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        return new EasySocketAuthenticationResult(Authenticated, RoomId, ClientId);
    }
}