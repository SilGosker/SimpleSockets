using EasySockets.Authentication;

namespace TestApplication.Authenticators;

public class ChatAuthenticators : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        if (context.Request.Query.TryGetValue("slug", out string slug))
        {

        }
    }
}