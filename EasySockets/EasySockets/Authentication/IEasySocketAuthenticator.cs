using Microsoft.AspNetCore.Http;

namespace EasySockets.Authentication;

/// <summary>
///     The interface for authenticating a client.
/// </summary>
public interface IEasySocketAuthenticator
{
    /// <summary>
    ///     Authenticates the client.
    /// </summary>
    /// <returns>
    ///     The authentication result that will be passed into the next authenticator or used as the definitive result
    /// </returns>
    /// <inheritdoc cref="IEasySocketAsyncAuthenticator.AuthenticateAsync" />
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult, HttpContext context);
}