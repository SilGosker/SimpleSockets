namespace SimpleSockets.Enums;

public enum BroadCastLevel
{
    /// <summary>
    /// Broadcasts to no one
    /// </summary>
    None = 0,
    /// <summary>
    /// Broadcasts to the users matching the <see cref="SimpleSocket.RoomId"/> of the sender, excluding the sender
    /// </summary>
    RoomMembers = 1,
    /// <summary>
    /// Broadcasts to the users matching the <see cref="SimpleSocket.RoomId"/> of the sender, including the sender
    /// </summary>
    Room = 2,
    /// <summary>
    /// Broadcasts to the users matching the senders type, excluding the sender
    /// </summary>
    TypeMembers = 3,
    /// <summary>
    /// Broadcasts to the users matching the senders type, including the sender
    /// </summary>
    Type = 4,
    /// <summary>
    /// Broadcasts to all users connected to the server, excluding the sender
    /// </summary>
    Members = 5,
    /// <summary>
    /// Broadcasts to the users connected to the server, including the sender
    /// </summary>
    EveryOne = 6
}