using System.Text;
using EasySockets.Authentication;

namespace EasySockets.Builder;

public sealed class EasySocketOptions
{
    private int _bufferSize = 100;

    private string _closingStatusDescription = "Closing";

    private Encoding _encoding = Encoding.UTF8;
    private int _receiveBufferSize = 100;
    private int _sendBufferSize = 100;
    internal List<Type> Authenticators { get; set; } = new();

    /// <summary>
    ///     The size of chunks when receiving or sending messages. <br />
    ///     Default is 100 bytes (100B).
    /// </summary>
    [Obsolete($"Use {nameof(ReceiveBufferSize)} and {nameof(SendBufferSize)} instead")]
    public int BufferSize
    {
        get => _bufferSize;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BufferSize)} cannot be less than 1");
            _bufferSize = value;
            _receiveBufferSize = value;
            _sendBufferSize = value;
        }
    }

    /// <summary>
    ///     The size of the byte array used to receive messages.<br />
    ///     When expecting to receive large messages, it is recommended to increase this value.<br />
    ///     Default is 100 bytes (100B).
    /// </summary>
    public int ReceiveBufferSize
    {
        get => _receiveBufferSize;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"{nameof(ReceiveBufferSize)} cannot be less than 1");
            _receiveBufferSize = value;
        }
    }

    /// <summary>
    ///     The size of the byte array used to send messages.<br />
    ///     When expecting to send large messages back to the client, it is recommended to increase this value.<br />
    ///     Default is 100 bytes (100B).
    /// </summary>
    public int SendBufferSize
    {
        get => _sendBufferSize;
        set
        {
            if (value < 1)
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SendBufferSize)} cannot be less than 1");
            _sendBufferSize = value;
        }
    }

    /// <summary>
    ///     The encoding that will be used to encode en decode messages from and to bytes. <br /><br />
    ///     The default is <see cref="Encoding.UTF8" />
    /// </summary>
    public Encoding Encoding
    {
        get => _encoding;
        set => _encoding = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     A human readable description as to why the socket is closing or is closed. <br /><br />
    ///     The default is <c>"Closing"</c>.
    /// </summary>
    public string ClosingStatusDescription
    {
        get => _closingStatusDescription;
        set => _closingStatusDescription = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     Adds a single authenticator to the sockets authentication pipeline.
    /// </summary>
    /// <typeparam name="TAuthenticator">The type of the authenticator</typeparam>
    public void AddAuthenticator<TAuthenticator>()
        where TAuthenticator : class, IEasySocketAuthenticator
    {
        Authenticators.Add(typeof(TAuthenticator));
    }

    /// <summary>
    ///     Adds a single async authenticator to the sockets authentication pipeline.
    /// </summary>
    /// <typeparam name="TAuthenticator">The type of the authenticator</typeparam>
    public void AddAsyncAuthenticator<TAuthenticator>()
        where TAuthenticator : class, IEasySocketAsyncAuthenticator
    {
        Authenticators.Add(typeof(TAuthenticator));
    }
}