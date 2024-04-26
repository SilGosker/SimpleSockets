using EasySockets.Authentication;

namespace EasySocketAuthenticatedChat.Authenticators;

public class TokenAuthenticator : IEasySocketAuthenticator
{
    private readonly IConfiguration _configuration;

    public TokenAuthenticator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("token", out var token))
        {
            return false;
        }

        if (token != _configuration["EasySockets:Token"])
        {
            return false;
        }

        return true;
    }
}