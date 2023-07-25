using SimpleSockets.Interfaces;

namespace SimpleSockets.Options;

public class SimpleSocketOptions
{
    /// <summary>
    /// Whether or not to chunk messages when receiving them.
    /// 
    /// Default is true
    /// </summary>
    public bool EnableChunkedMessages { get; set; } = true;
    /// <summary>
    /// The size of chunks when receiving messages. Irrelevant if <see cref="EnableChunkedMessages"/> is false.
    ///
    /// Default is 1024
    /// </summary>
    public int ChunkSize { get; set; } = 1024;
    /// <summary>
    /// The maximum size of the message when receiving one.
    /// 
    /// Default is 10KB (10240 bytes)
    /// </summary>
    public int MaxMessageSize { get; set; } = 1024 * 10;
    /// <summary>
    /// Whether or not this socket is authenticated at default.
    /// If not null, overrides the <see cref="SimpleSocketMiddlewareOptions.IsDefaultAuthenticated"/> property.
    /// Does not override the use of an authenticator.
    ///
    /// Default is null
    /// </summary>
    public bool? IsDefaultAuthenticated { get; set; } = null;

    internal Type? AuthenticatorType;
    public void UseAuthenticator<TAuthenticator>()
    where TAuthenticator : ISimpleSocketAuthenticator
    {
        AuthenticatorType = typeof(TAuthenticator);
    }
    
}