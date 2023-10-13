using EasySockets.Builder;

namespace EasySockets.Authentication;

/// <summary>
///     The interface for authenticating a client asynchronously.
/// </summary>
public interface IEasySocketAsyncAuthenticator
{
    /// <summary>
    ///     Authenticates the client asynchronously.
    /// </summary>
    /// <param name="previousAuthenticationResult">
    ///     <para>
    ///         The authentication result returned by the previous authenticator if available. Otherwise, the default
    ///         authentication result. For more information about the authentication and authorization, see <see href="https://github.com/SilGosker/EasySockets#Authentication">Authentication</see>
    ///     </para>
    ///     <para>
    ///         The default authentication result can configured through the
    ///         <see
    ///             cref="AppBuilderExtensions.UseEasySockets(Microsoft.AspNetCore.Builder.IApplicationBuilder, Action{EasySocketMiddlewareOptions})" />
    ///         and the <see cref="EasySocketBuilder.AddEasySocket(string,Type, Action{EasySocketOptions})" />.
    ///     </para>
    /// </param>
    /// <returns>
    ///     A task that represents the authentication result
    ///     that will be passed into the next authenticator or used as the definitive result
    /// </returns>
    public Task<EasySocketAuthenticationResult> AuthenticateAsync(
        EasySocketAuthenticationResult previousAuthenticationResult);
}