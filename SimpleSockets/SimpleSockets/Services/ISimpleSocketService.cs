namespace SimpleSockets.Services;

public interface ISimpleSocketService
{
    /// <summary>
    /// Checks if a user is connected and available
    /// </summary>
    /// <param name="roomId">The room id of the socket</param>
    /// <param name="userId">The user id of the socket</param>
    /// <returns>Whether the user is found and connected</returns>
    public bool Any(string roomId, string userId);
    /// <summary>
    /// Checks if a room is found and available
    /// </summary>
    /// <param name="roomId">The room id of the socket</param>
    /// <returns>Whether the room is found and at least 1 socket is connected</returns>
    public bool Any(string roomId);
    /// <summary>
    /// Forces a room to be removed and instantiates the leave event for all users in the room
    /// </summary>
    /// <param name="roomId">The room id</param>
    /// <returns>The task representing the operation of the leave event and removal</returns>
    public Task ForceLeave(string roomId);
    /// <summary>
    /// Forces a user to leave a room and instantiates the leave event 
    /// </summary>
    /// <param name="roomId">The room id the user is found in</param>
    /// <param name="userId">The user id of the socket in the room</param>
    /// <returns>The task representing the operation of the leave event and removal</returns>
    public Task ForceLeave(string roomId, string userId);
    /// <summary>
    /// Sends a message to all the users in a room with an event id or name
    /// </summary>
    /// <param name="roomId">The room id in which the users need to receive the messages</param>
    /// <param name="event">The event id or name</param>
    /// <param name="message">The message itself</param>
    /// <returns>The task representing the parallel sending of the messages</returns>
    public Task SendToRoom(string roomId, string @event, string message);
    /// <summary>
    /// Sends a message to all the users in a room
    /// </summary>
    /// <param name="roomId">The room id in which the users need to receive the messages</param>
    /// <param name="message">The message</param>
    /// <returns>The task representing the parallel sending of the messages</returns>
    public Task SendToRoom(string roomId, string message);
    /// <summary>
    /// Sends a message to a user in a room with an event id or name
    /// </summary>
    /// <param name="roomId">The room id in which the user need to receive the message</param>
    /// <param name="userId">The user id which needs to receive the message</param>
    /// <param name="event">The event id or name</param>
    /// <param name="message">The message</param>
    /// <returns>The task representing the sending of the message</returns>
    public Task SendToUser(string roomId, string userId, string @event, string message);
    /// <summary>
    /// Sends a message to a user in a room
    /// </summary>
    /// <param name="roomId">The room id in which the user needs to receive the message</param>
    /// <param name="userId">The user id which needs to receive the message</param>
    /// <param name="message">The message</param>
    /// <returns>The task representing the sending of the message</returns>
    public Task SendToUser(string roomId, string userId, string message);

}