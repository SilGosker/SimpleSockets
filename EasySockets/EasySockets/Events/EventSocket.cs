using System.Text.Json;
using EasySockets.Enums;

namespace EasySockets.Events;

public abstract class EventSocket : EventSocket<EasySocketEvent>
{
    /// <param name="event">The event to broadcast</param>
    /// <inheritdoc cref="EventSocket{TEvent}.Broadcast(string,string)"/>
    public Task Broadcast(EasySocketEvent @event)
    {
        return Broadcast(@event.Event, @event.Message);
    }

    /// <param name="event">The event to broadcast</param>
    /// <inheritdoc cref="EventSocket{TEvent}.Broadcast(BroadCastFilter,string,string)"/>
    public Task Broadcast(BroadCastFilter filter, EasySocketEvent @event)
    {
        return Broadcast(filter, @event.Event, @event.Message);
    }

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