namespace SimpleSockets.Enums;

public enum BroadCastLevel
{
    /// <summary>
    /// Broadcasts to no one
    /// </summary>
    None = 0,
    /// <summary>
    /// Broadcasts to the users matching the <see cref="SimpleSocket.RoomId"/> of the sender except for the sender
    /// </summary>
    RoomMembers = 1,
    /// <summary>
    /// Broadcasts to the users matching the <see cref="SimpleSocket.RoomId"/>
    /// </summary>
    Room = 2,
    /// <summary>
    /// Broadcasts to the users matching the senders type except for the sender
    /// </summary>
    TypeMembers = 3,
    /// <summary>
    /// Broadcasts to the users connected to the server except for the sender itself
    /// </summary>
    AllTypes = 4,
    /// <summary>
    /// Broadcasts to the users matching the <see cref="SimpleSocket.RoomId"/> of the sender
    /// </summary>
    Members = 5,
    /// <summary>
    /// Broadcasts to the users connected to the server
    /// </summary>
    EveryOne = 6
}