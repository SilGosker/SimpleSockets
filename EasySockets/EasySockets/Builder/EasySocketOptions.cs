using System.Text;
using EasySockets.Authentication;

namespace EasySockets.Builder;

public class EasySocketOptions
{
	private int _chunkSize = 100;

	private string _closingStatusDescription = "Closing";

	private Encoding _encoding = Encoding.UTF8;
	internal List<Type> Authenticators { get; set; } = new();

	/// <summary>
	///     The size of chunks when receiving messages. <br /><br />
	///     Increasing this will allocate more memory for each authenticated request. (Not for each message) <br />
	///     Decreasing this will cause receiving the full message to take more time.<br /><br />
	///     Default is 100 bytes (100B).
	/// </summary>
	public int ChunkSize
	{
		get => _chunkSize;
		set
		{
			if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Chunk size cannot be less than 0");
			_chunkSize = value;
		}
	}

    /// <summary>
    ///     Whether or not this socket is authenticated by default. <br /><br />
    ///     overrides the <see cref="EasySocketMiddlewareOptions" />.
    ///     <see cref="EasySocketMiddlewareOptions.IsDefaultAuthenticated" /> property if not null.
    ///     <br />
    ///     Does not override the use of an authenticator.<br /><br />
    ///     If not null, the value of the first <see cref="EasySocketAuthenticationResult" />.
    ///     <see cref="EasySocketAuthenticationResult.IsAuthenticated" /> will be
    ///     the value this property.<br /><br />
    ///     The default is null.
    /// </summary>
    public bool? IsDefaultAuthenticated { get; set; } = null;
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
		where TAuthenticator : IEasySocketAuthenticator
	{
		Authenticators.Add(typeof(TAuthenticator));
	}

	/// <summary>
	///     Adds a single async authenticator to the sockets authentication pipeline.
	/// </summary>
	/// <typeparam name="TAuthenticator">The type of the authenticator</typeparam>
	public void AddAsyncAuthenticator<TAuthenticator>()
		where TAuthenticator : IEasySocketAsyncAuthenticator
	{
		Authenticators.Add(typeof(TAuthenticator));
	}
}