using System.Net.WebSockets;
using EasySockets.Builder;
using EasySockets.Enums;

namespace EasySockets;

public interface IInternalEasySocket
{
    internal string RoomId { set; }
    internal string ClientId { set; }
    internal WebSocket WebSocket { set; }
    internal EasySocketOptions Options { set; }
	internal Func<IEasySocket, BroadCastFilter, string, Task>? Emit { set; }
    internal Action<IEasySocket>? DisposeAtSocketHandler { set; }
}