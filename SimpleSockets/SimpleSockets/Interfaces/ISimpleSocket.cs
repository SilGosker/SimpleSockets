using SimpleSockets.Enums;

namespace SimpleSockets.Interfaces;

public interface ISimpleSocket
{
    internal Task ReceiveMessages();
    internal bool IsConnected();
    public string RoomId { get; internal set; }
    public string UserId { get; internal set; }
    internal Task OnConnect();
    internal Task Leave();
    internal Task SendMessage(string message);
    internal Task SendMessage(string @event, string message);
    internal Func<ISimpleSocket, BroadCastLevel, string, Task>? Emit { get; set; }
    internal Action<ISimpleSocket>? DisposeAtSocketHandler { get; set; }
}