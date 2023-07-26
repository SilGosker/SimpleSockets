using System.Text;
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
    /// The Default is null
    /// </summary>
    public bool? IsDefaultAuthenticated { get; set; } = null;

    internal Type? AuthenticatorType;
    /// <summary>
    /// The encoding that will be used to encode en decode messages from and to bytes.
    ///
    /// The default is <see cref="System.Text.Encoding.UTF8"/>
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    public void UseAuthenticator<TAuthenticator>()
    where TAuthenticator : ISimpleSocketAuthenticator
    {
        AuthenticatorType = typeof(TAuthenticator);
    }
    
    public void UseAsyncAuthenticator<TAuthenticator>()
        where TAuthenticator : ISimpleSocketAsyncAuthenticator
    {
        AuthenticatorType = typeof(TAuthenticator);
    }
}