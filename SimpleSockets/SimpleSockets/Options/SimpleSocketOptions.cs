using System.Text;
using SimpleSockets.Authentication;

namespace SimpleSockets.Options;

public class SimpleSocketOptions
{
    internal List<Type> Authenticators = new();
    
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

    /// <summary>
    /// The encoding that will be used to encode en decode messages from and to bytes.
    ///
    /// The default is <see cref="System.Text.Encoding.UTF8"/>
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;
    
    /// <summary>
    /// Adds a single authenticator to the sockets authentication pipeline.
    /// </summary>
    /// <typeparam name="TAuthenticator">The type of the authenticator</typeparam>
    public void AddAuthenticator<TAuthenticator>()
    where TAuthenticator : ISimpleSocketAuthenticator
    {
        Authenticators.Add(typeof(TAuthenticator));
    }
    
    /// <summary>
    /// Adds a single async authenticator to the sockets authentication pipeline.
    /// </summary>
    /// <typeparam name="TAuthenticator">The type of the authenticator</typeparam>
    public void AddAsyncAuthenticator<TAuthenticator>()
        where TAuthenticator : ISimpleSocketAsyncAuthenticator
    {
        Authenticators.Add(typeof(TAuthenticator));
    }

    /// <summary>
    /// Adds multiple authenticators to the socket.
    /// Types may contain both async and non-async authenticators.
    /// </summary>
    /// <param name="authenticators">The authenticators that at least implement the <see cref="ISimpleSocketAuthenticator"/> or the <see cref="ISimpleSocketAsyncAuthenticator"/> interfaces</param>
    /// <exception cref="ArgumentException">When the authenticator doesn't implement either the <see cref="ISimpleSocketAuthenticator"/> or the <see cref="ISimpleSocketAsyncAuthenticator"/> interfaces</exception>
    public void AddAuthenticatorRange(params Type[] authenticators)
    {
        foreach (var authenticator in authenticators)
        {
            var interfaces = authenticator.GetInterfaces();
            if (!interfaces.Contains(typeof(ISimpleSocketAsyncAuthenticator)) && !interfaces.Contains(typeof(ISimpleSocketAuthenticator)))
            {
                throw new ArgumentException(nameof(authenticator), $"{authenticator.FullName} must implement the {typeof(ISimpleSocketAuthenticator).FullName} or the {typeof(ISimpleSocketAsyncAuthenticator).FullName} interfaces");
            }
        }
    }
}