using EasySockets;
using EasySockets.Enums;

namespace EasySocketBasicChat.Sockets;

public class ChatSocket : EasySocket
{
    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, $"Welcome to '{ClientId}' for joining the chat!");
    }

    public override Task OnDisconnect()
    {
        return Broadcast($"User '{ClientId}' has left the chat!");
    }
}