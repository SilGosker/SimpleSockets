using EasySockets.Authentication;
using EasySockets.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EasySockets.Builder;

public sealed class EasySocketGlobalOptions
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
        set
        {
            ThrowHelper.ThrowIfNull(value);
            _getDefaultRoomId = value;
        }
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
        set
        {
            ThrowHelper.ThrowIfNull(value);
            _getDefaultClientId = value;
        }
    }

    /// <summary>
    ///     The <see cref="Microsoft.AspNetCore.Builder.WebSocketOptions" /> that will be used in the middleware.
    /// </summary>
    public WebSocketOptions WebSocketOptions
    {
        get => _webSocketOptions;
        set
        {
            ThrowHelper.ThrowIfNull(value);
            _webSocketOptions = value;
        }
    }
}