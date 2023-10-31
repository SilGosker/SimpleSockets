namespace EasySockets.Events;

public class EasySocketEvent : IEasySocketEvent
{
    public static implicit operator EasySocketEvent(string @event)
    {
        return new EasySocketEvent(@event);
    }

    public EasySocketEvent()
    {
    }

    public EasySocketEvent(string @event)
    {
        Event = @event;
    }

    public EasySocketEvent(string @event, string message)
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