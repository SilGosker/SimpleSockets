using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SimpleSockets.Options;

public class SimpleSocketMiddlewareOptions
{
    /// <summary>
    /// The default way of getting a RoomId that will be used if no authenticator is provided.
    ///
    /// The default is always '__0'
    /// </summary>
    public Func<HttpContext, string> GetDefaultRoomId { get; set; } = _ => "__0";
    /// <summary>
    /// The default way of getting a new userId that will be used if no authenticator is provided.
    ///
    /// The default is a random Guid.
    /// </summary>
    public Func<HttpContext, string> GetDefaultUserId { get; set; } = _ => Guid.NewGuid().ToString();
    /// <summary>
    /// Whether the user is authenticated by default if no authenticator is used.
    ///
    /// The default is true.
    /// </summary>
    public bool IsDefaultAuthenticated { get; set; } = true;
    /// <summary>
    /// Whether or not to log authentication results.
    ///
    /// The default is true
    /// </summary>
    public bool EnableLogging { get; set; } = true;
    /// <summary>
    /// The <see cref="Microsoft.Extensions.Logging.LogLevel"/> that will be used through the middleware.
    /// 
    /// The default is <see cref="LogLevel.Information"/>
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// The <see cref="WebSocketOptions"/> that will be used through the middleware.
    /// </summary>
    public WebSocketOptions WebSocketOptions { get; set; } = new();
}