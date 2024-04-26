using EasySockets.Authentication;

namespace Authentication.Authenticators;

public class ChatAuthenticator : IEasySocketAuthenticator
{
	public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
		HttpContext context)
	{
		if (!context.Request.Query.TryGetValue("slug", out var slug))
		{
			return false;
		}

		return new EasySocketAuthenticationResult(true, slug, Random.Shared.Next().ToString());
	}
}