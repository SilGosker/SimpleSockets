using EasySockets.Events;

namespace CustomEventTypes.Events;

public class XmlEvent : IEasySocketEvent
{
    public string Event { get; set; } = null!;
    public string Message { get; set; } = null!;
}