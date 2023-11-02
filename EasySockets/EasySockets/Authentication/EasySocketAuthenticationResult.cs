namespace EasySockets.Authentication;

/// <summary>
///     The combined result of authentication and authorization for a <see cref="EasySocket" /> connection
/// </summary>
public struct EasySocketAuthenticationResult
{
	/// <inheritdoc cref="EasySocketAuthenticationResult(bool, string, string)" />
	public EasySocketAuthenticationResult(bool isAuthenticated)
		: this(isAuthenticated, null, null)
	{
	}

	/// <inheritdoc cref="EasySocketAuthenticationResult(bool, string, string)" />
	public EasySocketAuthenticationResult(bool isAuthenticated, string? roomId)
		: this(isAuthenticated, roomId, null)
	{
	}

	/// <summary>
	///     Creates a new <see cref="EasySocketAuthenticationResult" /> instance with the given parameters
	/// </summary>
	/// <param name="isAuthenticated">
	///     <para>
	///         Whether the authentication result indicated success or failure.
	///     </para>
	///     <para>
	///         Failure (<c>false</c>) will result in a 401 status code. <br />
	///         Success (<c>true</c>) will result in the websocket being accepted.
	///     </para>
	/// </param>
	/// <param name="roomId">The room identifier the client joins after connecting</param>
	/// <param name="clientId">The unique client identifier.</param>
	public EasySocketAuthenticationResult(bool isAuthenticated, string? roomId, string? clientId)
	{
		IsAuthenticated = isAuthenticated;
		ClientId = clientId;
		RoomId = roomId;
	}

	/// <summary>
	///     <para>
	///         Whether the authentication result indicated success or failure.
	///     </para>
	///     <para>
	///         Failure (<c>false</c>) will result in a 401 status code. <br />
	///         Success (<c>true</c>) will result in the websocket being accepted.
	///     </para>
	/// </summary>
	public bool IsAuthenticated { get; set; }

	/// <summary>
	///     <para>
	///         The unique client identifier.<br />
	///         The identifier should be unique <b>per room</b>.<br />
	///         This means a client can have a duplicate identifier if they are in different rooms.
	///     </para>
	/// </summary>
	public string? ClientId { get; set; }

	/// <summary>
	///     <para>
	///         The room id the client joins when successfully authenticated
	///     </para>
	/// </summary>
	public string? RoomId { get; set; }

	/// <summary>
	///     Implicitly converts a <see cref="bool" /> into a <see cref="EasySocketAuthenticationResult" /> with the
	///     <see cref="IsAuthenticated" /> set to the indicated value
	/// </summary>
	/// <param name="authenticationResult">Whether the result should indicate success or failure</param>
	public static implicit operator EasySocketAuthenticationResult(bool authenticationResult)
	{
		return new EasySocketAuthenticationResult(authenticationResult);
	}

	/// <summary>
	///     Implicitly converts a <see cref="EasySocketAuthenticationResult" /> into a <see cref="bool" />  with the
	///     <see cref="IsAuthenticated" /> value as the result
	/// </summary>
	public static implicit operator bool(EasySocketAuthenticationResult? authenticationResult)
	{
		return authenticationResult?.IsAuthenticated == true;
	}
}