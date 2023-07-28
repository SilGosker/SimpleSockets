namespace SimpleSockets.Authentication;

/// <summary>
///     The interface for authenticating a client.
/// </summary>
public interface ISimpleSocketAuthenticator
{
    /// <summary>
    ///     Authenticates the client.
    /// </summary>
    /// <returns>The authentication result.</returns>
    /// <inheritdoc cref="ISimpleSocketAsyncAuthenticator.AuthenticateAsync" />
    public SimpleSocketAuthenticationResult Authenticate(SimpleSocketAuthenticationResult previousAuthenticationResult);
}