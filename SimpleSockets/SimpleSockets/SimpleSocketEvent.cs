using SimpleSockets.Interfaces;

namespace SimpleSockets;

public class SimpleSocketEvent : ISimpleSocketEvent
{
    public static implicit operator SimpleSocketEvent(string @event)
    {
        return new SimpleSocketEvent(@event);
    }

    public SimpleSocketEvent()
    {
    }

    public SimpleSocketEvent(string @event)
    {
        Event = @event;
    }

    public SimpleSocketEvent(string @event, string message)
    {
        Event = @event;
        Message = message;
    }

    public string Event { get; set; } = null!;
    public string Message { get; set; } = null!;

    public string GetEvent()
    {
        return Event;
    }
}