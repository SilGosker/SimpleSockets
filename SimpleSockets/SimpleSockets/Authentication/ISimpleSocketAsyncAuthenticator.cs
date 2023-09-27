using SimpleSockets.Builder;

namespace SimpleSockets.Authentication;

/// <summary>
///     The interface for authenticating a client asynchronously.
/// </summary>
public interface ISimpleSocketAsyncAuthenticator
{
    /// <summary>
    ///     Authenticates the client asynchronously.
    /// </summary>
    /// <param name="previousAuthenticationResult">
    ///     <para>
    ///         The authentication result returned by the previous authenticator if available. Otherwise, the default
    ///         authentication result. For more information about the authentication and authorization, see <see href="https://github.com/SilGosker/SimpleSockets#Authentication">Authentication</see>
    ///     </para>
    ///     <para>
    ///         The default authentication result can configured through the
    ///         <see
    ///             cref="AppBuilderExtensions.UseSimpleSockets(Microsoft.AspNetCore.Builder.IApplicationBuilder, Action{SimpleSocketMiddlewareOptions})" />
    ///         and the <see cref="SimpleSocketBuilder.AddSimpleSocket(string,Type, Action{SimpleSocketOptions})" />.
    ///     </para>
    /// </param>
    /// <returns>
    ///     A task that represents the authentication result
    ///     that will be passed into the next authenticator or used as the definitive result
    /// </returns>
    public Task<SimpleSocketAuthenticationResult> AuthenticateAsync(
        SimpleSocketAuthenticationResult previousAuthenticationResult);
}