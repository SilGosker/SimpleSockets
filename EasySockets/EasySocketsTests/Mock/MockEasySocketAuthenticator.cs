using EasySockets.Authentication;
using Microsoft.AspNetCore.Http;

namespace EasySockets.Mock;

public class MockEasySocketAuthenticator : IEasySocketAuthenticator
{
    internal static bool Authenticated = true;
    internal const string RoomId = "MockEasySocketAuthenticator_TestRoomId";
    internal const string ClientId = "MockEasySocketAuthenticator_TestClientId";
    internal static int ConstructorCallCount { get; set; }

    public MockEasySocketAuthenticator()
    {
        ConstructorCallCount++;
    }
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        return new EasySocketAuthenticationResult(Authenticated, RoomId, ClientId);
    }
}