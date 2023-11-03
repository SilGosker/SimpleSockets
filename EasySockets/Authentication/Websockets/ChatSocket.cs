using System.Net.WebSockets;
using EasySockets;
using EasySockets.Builder;
using EasySockets.Enums;

namespace Authentication.Websockets;

public class ChatSocket : EasySocket
{
    public ChatSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {

    }

    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, $"Welcome {ClientId}. You are currently in room '{RoomId}'");
    }
}