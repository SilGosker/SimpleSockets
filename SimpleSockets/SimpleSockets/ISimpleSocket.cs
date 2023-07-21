using SimpleSockets.DataModels;
using SimpleSockets.Enums;

namespace SimpleSockets;

public interface ISimpleSocket
{
    internal Task ReceiveMessages();
    internal bool IsConnected();
    internal string RoomId { get; set; }
    internal string UserId { get; set; }
    internal Task OnConnect();
    internal Task Leave();
    internal Task SendMessage(string message);
    internal Task SendMessage(string @event, string message);
    internal Func<ISimpleSocket, BroadCastLevel, string, Task>? Emit { get; set; }
    internal Action<ISimpleSocket>? DisposeAtSocketHandler { get; set; }
}