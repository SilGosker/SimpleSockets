namespace EasySockets.Services;

public interface IEasySocketService
{
    /// <summary>
    ///     Checks if any client is connected.
    /// </summary>
    /// <returns><c>true</c> if a client is connected to the server, otherwise <c>false</c>.</returns>
    public bool Any();

    /// <summary>
    ///     Checks if a connected client is found matching the <paramref name="roomId" />.
    /// </summary>
    /// <param name="roomId">The room identifier the client needs to match.</param>
    /// <returns><c>true</c> if a client is found matching the prerequisite, otherwise <c>false</c>.</returns>
    public bool Any(string roomId);

    /// <summary>
    ///     Checks if a connected client specified by the <paramref name="roomId" /> and <paramref name="clientId" /> is found.
    /// </summary>
    /// <param name="roomId">The room identifier the client needs to match.</param>
    /// <param name="clientId">The identifier of the client.</param>
    /// <returns><c>true</c> if a client is found matching the prerequisite, otherwise <c>false</c>.</returns>
    public bool Any(string roomId, string clientId);

    /// <summary>
    ///     Counts the total amount of clients connected to the server.
    /// </summary>
    /// <returns>The amount of clients that are connected to the server.</returns>
    public int Count();

    /// <summary>
    ///     Counts the amount of clients connected to a room.
    /// </summary>
    /// <param name="roomId">The room identifier the client needs to match.</param>
    /// <returns>The amount of clients that match the rooms identifier.</returns>
    public int Count(string roomId);

    /// <inheritdoc cref="ForceLeaveAsync(string, CancellationToken)" />
    public Task ForceLeaveAsync(string roomId);

    /// <summary>
    ///     Instantiates the leave event and removes all clients in the room specified by the <paramref name="roomId" />.
    /// </summary>
    /// <inheritdoc cref="ForceLeaveAsync(string, string, CancellationToken)" />
    public Task ForceLeaveAsync(string roomId, CancellationToken cancellationToken);

    /// <inheritdoc cref="ForceLeaveAsync(string, string, CancellationToken)" />
    public Task ForceLeaveAsync(string roomId, string clientId);

    /// <summary>
    ///     Instantiates the leave event and removes the client specified by the <paramref name="roomId" /> and
    ///     <paramref name="clientId" />.
    /// </summary>
    /// <param name="roomId">The room identifier the client needs to match to instantiate the leave event.</param>
    /// <param name="clientId">The client identifier the client needs to match to instantiate the leave event.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken" /> to cancel the asynchronous operation.</param>
    /// <returns>The task representing the asynchronous operation of the leave event and removal.</returns>
    public Task ForceLeaveAsync(string roomId, string clientId, CancellationToken cancellationToken);

    /// <inheritdoc cref="SendToRoomAsync(string, string, CancellationToken)" />
    public Task SendToRoomAsync(string roomId, string message);

    /// <summary>
    ///     Sends a message to all the clients in a room.
    /// </summary>
    /// <inheritdoc cref="SendToRoomAsync(string, string, string, CancellationToken)" />
    public Task SendToRoomAsync(string roomId, string message, CancellationToken cancellationToken);

    /// <inheritdoc cref="SendToRoomAsync(string, string, string, CancellationToken)" />
    public Task SendToRoomAsync(string roomId, string @event, string message);

    /// <summary>
    ///     Sends a message to all the clients in a room with an event.
    /// </summary>
    /// <param name="roomId">The room identifier the client needs to match to receive the message.</param>
    /// <param name="event">The event identifier or name.</param>
    /// <param name="message">The message to be sent to the clients.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken" /> to cancel the asynchronous operation.</param>
    /// <returns>The task representing the asynchronous parallel sending of the messages.</returns>
    public Task SendToRoomAsync(string roomId, string @event, string message, CancellationToken cancellationToken);

    /// <inheritdoc cref="SendToClientAsync(string, string, string, CancellationToken)" />
    public Task SendToClientAsync(string roomId, string clientId, string message);

    /// <summary>
    ///     Sends a message to a client.
    /// </summary>
    /// <inheritdoc cref="SendToClientAsync(string, string, string, string, CancellationToken)" />
    public Task SendToClientAsync(string roomId, string clientId, string message, CancellationToken cancellationToken);

    /// <inheritdoc cref="SendToClientAsync(string, string, string, string, CancellationToken)" />
    public Task SendToClientAsync(string roomId, string clientId, string @event, string message);

    /// <summary>
    ///     Sends a message to a client in a room with an event.
    /// </summary>
    /// <param name="roomId">The room identifier the client needs to match to receive the message.</param>
    /// <param name="clientId">The identifier the client should match to receive the message.</param>
    /// <param name="event">The event identifier or name.</param>
    /// <param name="message">The message to be sent to the client.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken" /> to cancel the operation the message sending.</param>
    /// <returns>The task representing the sending of the message.</returns>
    public Task SendToClientAsync(string roomId, string clientId, string @event, string message,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Lists all rooms and their clients.
    /// </summary>
    /// <returns>
    ///     An enumerator that iterates through all rooms and their clients grouped based on the room's identifier.
    /// </returns>
    public IEnumerable<IGrouping<string, IEasySocket>> GetGroupings();
}