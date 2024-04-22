
namespace EasySockets.Events;

public sealed class EasySocketEvent : IEasySocketEvent
{
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
}