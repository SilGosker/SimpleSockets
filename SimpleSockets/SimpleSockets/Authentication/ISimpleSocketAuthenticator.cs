using SimpleSockets.Options;

namespace SimpleSockets.Authentication;

public interface ISimpleSocketAuthenticator
{
    /// <summary>
    /// Authenticates the socket.
    /// </summary>
    /// <param name="previousAuthenticationResult">The previous authentication result, if available. Otherwise, the default authentication result.
    ///
    /// The default <see cref="SimpleSocketAuthenticationResult.IsAuthenticated"/> is based on the
    /// <see cref="SimpleSocketOptions.IsDefaultAuthenticated"/>. If null, the <see cref="SimpleSocketMiddlewareOptions.IsDefaultAuthenticated"/> is used.
    /// The default <see cref="SimpleSocketAuthenticationResult.UserId"/> and <see cref="SimpleSocketAuthenticationResult.RoomId"/> are both always null.
    /// </param>
    /// <returns>The authentication result</returns>
    public SimpleSocketAuthenticationResult Authenticate(SimpleSocketAuthenticationResult previousAuthenticationResult);
}