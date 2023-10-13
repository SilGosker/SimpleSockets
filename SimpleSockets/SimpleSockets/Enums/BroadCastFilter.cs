namespace EasySockets.Enums;

/// <summary>
/// Specifies which clients should and should not receive the message.
/// </summary>
[Flags]
public enum BroadCastFilter
{
    /// <summary>
    /// Broadcasts to everyone on the server.
    /// </summary>
    Everyone = 0,
    /// <summary>
    /// Adds the filter that the sender is excluded from the broadcast.
    /// </summary>
    Members = 1,
    /// <summary>
    /// Adds the filter that all clients should match the sender's type.
    /// </summary>
    EqualType = 2,
    /// <summary>
    /// Adds the filter that all clients should match the sender's <see cref="EasySocket.RoomId"/>.
    /// </summary>
    EqualRoomId = 4,
    /// <summary>
    /// Adds the filter that all clients should match the sender's <see cref="EasySocket.RoomId"/> excluding the sender.
    /// </summary>
    RoomMembers = Members | EqualRoomId,
    /// <summary>
    /// Adds the filter that all clients should match the sender's type excluding the sender.
    /// </summary>
    TypeMembers = Members | EqualType
}