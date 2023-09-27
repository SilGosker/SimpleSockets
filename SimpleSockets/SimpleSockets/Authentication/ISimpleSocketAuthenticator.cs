namespace SimpleSockets.Authentication;

/// <summary>
///     The interface for authenticating a client.
/// </summary>
public interface ISimpleSocketAuthenticator
{
    /// <summary>
    ///     Authenticates the client.
    /// </summary>
    /// <returns>
    ///     The authentication result that will be passed into the next authenticator or used as the definitive result
    /// </returns>
    /// <inheritdoc cref="ISimpleSocketAsyncAuthenticator.AuthenticateAsync" />
    public SimpleSocketAuthenticationResult Authenticate(SimpleSocketAuthenticationResult previousAuthenticationResult);
}