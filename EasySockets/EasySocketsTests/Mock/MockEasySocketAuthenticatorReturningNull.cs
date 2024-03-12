using EasySockets.Authentication;
using Microsoft.AspNetCore.Http;

namespace EasySockets.Mock;

public class MockEasySocketAuthenticatorReturningNull : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        return true;
    }
}