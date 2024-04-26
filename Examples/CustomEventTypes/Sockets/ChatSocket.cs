    using CustomEventTypes.Events;
    using EasySockets.Enums;

    namespace CustomEventTypes.Sockets;

    public class ChatSocket : XmlEventSocket
    {
        public Task OnTyping()
        {
            return Broadcast("Typing", ClientId + " is typing...");
        }

        public Task OnMessage(XmlEvent @event)
        {
            return Broadcast(@event.Event, @event.Message);
        }

        public override Task OnConnect()
        {
            return Broadcast(BroadCastFilter.EqualRoomId, "Connected", $"Welcome {ClientId}. You are currently in room '{RoomId}'");
        }
    }