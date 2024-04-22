using EasySockets.Attributes;
using EasySockets.Enums;
using EasySockets.Events;

namespace EasySocketEvents.Sockets;

public class ChatSocket : EventSocket
{
    [InvokeOn("Typing")]
    [InvokeOn("typing")]
    public Task SomeoneTyped()
    {
        return Broadcast(new EasySocketEvent("Typing", ClientId + " is typing..."));
    }

    [InvokeOn("Message")]
    [InvokeOn("message")]
    public Task MessageReceived(EasySocketEvent @event)
    {
        return Broadcast(@event);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, new EasySocketEvent("Connected", $"Welcome {ClientId}. You are currently in room '{RoomId}'"));
    }
}