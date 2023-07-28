using SimpleSockets.Extensions;
using SimpleSockets.Options;

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
    ///         authentication result.
    ///     </para>
    ///     <para>
    ///         The default authentication result can configured through the
    ///         <see
    ///             cref="AppBuilderExtensions.UseSimpleSockets(Microsoft.AspNetCore.Builder.IApplicationBuilder, Action{SimpleSocketMiddlewareOptions})" />
    ///         and the <see cref="SimpleSocketBuilder.AddSimpleSocket(string,Type, Action{SimpleSocketOptions})" />.
    ///     </para>
    /// </param>
    /// <returns>A task representing the asynchronous authentication.</returns>
    public Task<SimpleSocketAuthenticationResult> AuthenticateAsync(
        SimpleSocketAuthenticationResult previousAuthenticationResult);
}