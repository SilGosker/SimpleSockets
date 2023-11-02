namespace EasySockets.Services;

public interface IEasySocketService
{
	/// <summary>
	///     Checks if a client is connected and available
	/// </summary>
	/// <param name="roomId">The room id of the socket</param>
	/// <param name="clientId">The client id of the socket</param>
	/// <returns>Whether the client is found and connected</returns>
	public bool Any(string roomId, string clientId);

	/// <summary>
	///     Checks if a room is found and available
	/// </summary>
	/// <param name="roomId">The room id of the socket</param>
	/// <returns>Whether the room is found and at least 1 socket is connected</returns>
	public bool Any(string roomId);

	/// <summary>
	///     Counts the amount of clients connected to a room
	/// </summary>
	/// <param name="roomId">The room id</param>
	/// <returns>The count on how many clients are in the room</returns>
	public int Count(string roomId);

	/// <summary>
	///     Counts the amount of clients connected to the server
	/// </summary>
	/// <returns>The count of how many clients are connected to the server</returns>
	public int Count();

	/// <summary>
	///     Forces a room to be removed and instantiates the leave event for all clients in the room
	/// </summary>
	/// <param name="roomId">The room id</param>
	/// <returns>The task representing the operation of the leave event and removal</returns>
	public Task ForceLeave(string roomId);

	/// <summary>
	///     Forces a client to leave a room and instantiates the leave event
	/// </summary>
	/// <param name="roomId">The room id the client is found in</param>
	/// <param name="clientId">The client id of the socket in the room</param>
	/// <returns>The task representing the operation of the leave event and removal</returns>
	public Task ForceLeave(string roomId, string clientId);

	/// <summary>
	///     Sends a message to all the clients in a room with an event id or name
	/// </summary>
	/// <param name="roomId">The room id in which the clients need to receive the messages</param>
	/// <param name="event">The event id or name</param>
	/// <param name="message">The message itself</param>
	/// <returns>The task representing the parallel sending of the messages</returns>
	public Task SendToRoom(string roomId, string @event, string message);

	/// <summary>
	///     Sends a message to all the clients in a room
	/// </summary>
	/// <param name="roomId">The room id in which the clients need to receive the messages</param>
	/// <param name="message">The message</param>
	/// <returns>The task representing the parallel sending of the messages</returns>
	public Task SendToRoom(string roomId, string message);

	/// <summary>
	///     Sends a message to a client in a room with an event id or name
	/// </summary>
	/// <param name="roomId">The room id in which the client need to receive the message</param>
	/// <param name="clientId">The client id which needs to receive the message</param>
	/// <param name="event">The event id or name</param>
	/// <param name="message">The message</param>
	/// <returns>The task representing the sending of the message</returns>
	public Task SendToClient(string roomId, string clientId, string @event, string message);

	/// <summary>
	///     Sends a message to a client in a room
	/// </summary>
	/// <param name="roomId">The room id in which the client needs to receive the message</param>
	/// <param name="clientId">The client id which needs to receive the message</param>
	/// <param name="message">The message</param>
	/// <returns>The task representing the sending of the message</returns>
	public Task SendToClient(string roomId, string clientId, string message);

	/// <summary>
	///     Lists all rooms and their clients
	/// </summary>
	/// <returns>
	///     An enumerator that iterates through all rooms and their clients<br />
	///     grouped based on the room's identifier
	/// </returns>
	public IEnumerable<IGrouping<string, IEasySocket>> GetGroups();
}