namespace SimpleSockets.Enums;
[Flags]
public enum BroadCastLevel
{
    /// <summary>
    /// Broadcasts to no one
    /// </summary>
    None = 0,
    /// <summary>
    /// Broadcasts to only the users in the same room as the sender matching the <see cref="SimpleSocket.RoomId"/> except for the sender itself
    /// </summary>
    Members = 5,
    /// <summary>
    /// Broadcasts to only the users in the same room as the sender matching the <see cref="SimpleSocket.RoomId"/>
    /// </summary>
    EveryOne = 10
}