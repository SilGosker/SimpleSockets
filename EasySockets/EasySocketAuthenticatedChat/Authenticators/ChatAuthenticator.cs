using EasySockets.Authentication;

namespace EasySocketAuthenticatedChat.Authenticators;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        if (context.Request.Query.TryGetValue("token", out var token) || token != "123")
        {
            return false;
        }

        if (!context.Request.Query.TryGetValue("room", out var roomId))
        {
            return false;
        }

        return new(true, roomId);
    }
}