using EasySockets.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EasySockets.Builder;

public class EasySocketMiddlewareOptions
{
    private Func<HttpContext, string> _getDefaultClientId = _ => Guid.NewGuid().ToString();
    private Func<HttpContext, string> _getDefaultRoomId = _ => "__0";
    private WebSocketOptions _webSocketOptions = new();

    /// <summary>
    ///     <para>
    ///         The default way of getting a RoomId that will be used if no authenticator is provided or if the definitive
    ///         <see cref="EasySocketAuthenticationResult.RoomId" /> is null.
    ///         <para>
    ///             If this method returns null, the middleware switches back to <c>"__0"</c>.
    ///         </para>
    ///     </para>
    ///     <para>
    ///         The default is a method that returns the <c>"__0"</c> string.
    ///     </para>
    /// </summary>
    public Func<HttpContext, string> GetDefaultRoomId
    {
        get => _getDefaultRoomId;
        set => _getDefaultRoomId = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     <para>
    ///         The default way of getting a new userId that will be used if no authenticator is provided or if the definitive
    ///         <see cref="EasySocketAuthenticationResult.ClientId" /> is null.
    ///         <para>
    ///             If returns null, the middleware switches back to <c>Guid.NewGuid()</c>.
    ///         </para>
    ///     </para>
    ///     <para>
    ///         The default is method that returns <c>Guid.NewGuid()</c>.
    ///     </para>
    /// </summary>
    public Func<HttpContext, string> GetDefaultClientId
    {
        get => _getDefaultClientId;
        set => _getDefaultClientId = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     <para>
    ///         Whether the client is authenticated by default
    ///         <para>
    ///             This option will only be used when no authenticator are used and if the configured
    ///             <see cref="EasySocketOptions.IsDefaultAuthenticated" /> is <c>null</c>
    ///             <br />
    ///             The <see cref="EasySocketOptions.IsDefaultAuthenticated" /> can be
    ///             configured
    ///             through the <see cref="AppBuilderExtensions.UseEasySockets" />
    ///         </para>
    ///     </para>
    ///     <para>
    ///         The default is <c>false</c>.
    ///     </para>
    /// </summary>
    public bool IsDefaultAuthenticated { get; set; } = false;

    /// <summary>
    ///     The <see cref="Microsoft.AspNetCore.Builder.WebSocketOptions" /> that will be used in the middleware.
    /// </summary>
    public WebSocketOptions WebSocketOptions
    {
        get => _webSocketOptions;
        set => _webSocketOptions = value ?? throw new ArgumentNullException(nameof(value));
    }
}