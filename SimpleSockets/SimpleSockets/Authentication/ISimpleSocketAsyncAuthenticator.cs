using SimpleSockets.Options;

namespace SimpleSockets.Authentication;

public interface ISimpleSocketAsyncAuthenticator
{
    /// <summary>
    /// Authenticates the socket asynchronously.
    /// </summary>
    /// <param name="previousAuthenticationResult">
    /// The previous authentication result, if available. Otherwise, the default authentication result.
    /// </param>
    /// <returns>A task representing the asynchronous authentication</returns>
    public Task<SimpleSocketAuthenticationResult> AuthenticateAsync(SimpleSocketAuthenticationResult previousAuthenticationResult);
}