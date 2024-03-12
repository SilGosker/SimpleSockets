using System.ComponentModel.DataAnnotations;

namespace EasySockets.Events;

public class EasySocketEvent : IEasySocketEvent
{

    public static implicit operator EasySocketEvent(string @event)
    {
        return new EasySocketEvent(@event);
    }

    public EasySocketEvent()
    {
        Event = string.Empty;
		Message = string.Empty;
    }

    public EasySocketEvent(string @event)
    {
        Event = @event;
        Message = string.Empty;
    }

    public EasySocketEvent(string @event, string message)
    {
        Event = @event;
        Message = message;
    }

    public string Event { get; set; }
    
    public string Message { get; set; }

    public string GetEvent()
    {
        return Event;
    }
}