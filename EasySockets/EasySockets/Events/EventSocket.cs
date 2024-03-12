using System.Text.Json;

namespace EasySockets.Events;

public abstract class EventSocket : EventSocket<EasySocketEvent>
{
    public sealed override EasySocketEvent? ExtractEvent(string message)
    {
        try
        {
            return JsonSerializer.Deserialize<EasySocketEvent>(message, EasySocketEventConverter.EasySocketEventSerializerOptions);
        }
        catch
        {
            return null;
        }
    }

    public sealed override string ExtractMessage(EasySocketEvent @event, string message)
    {
        return @event.Message;
    }

    public sealed override string? BindEvent(string @event, string message)
    {
        try
        {
            return JsonSerializer.Serialize(new EasySocketEvent(@event, message), EasySocketEventConverter.EasySocketEventSerializerOptions);
        }
        catch
        {
            return null;
        }
    }
}