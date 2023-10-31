using EasySockets.Authentication;
using EasySockets.Services;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    private readonly IEasySocketService _easySocketService;

    public ChatAuthenticator(IEasySocketService easySocketService)
    {
	    _easySocketService = easySocketService;
    }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("slug", out var slug))
        {
            return false;
        }

        return new EasySocketAuthenticationResult(true, slug, _easySocketService.Count(slug).ToString());
    }
}