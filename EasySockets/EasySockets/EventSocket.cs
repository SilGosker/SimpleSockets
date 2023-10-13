using System.Net.WebSockets;
using System.Text.Json;
using EasySockets.Builder;
using EasySockets.DataModels;

namespace EasySockets;

public abstract class EventSocket : EventSocket<EasySocketEvent>
{
    protected EventSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {
    }

    public sealed override Task<EasySocketEvent?> ExtractEvent(string message)
    {
        try
        {
            return Task.FromResult(JsonSerializer.Deserialize<EasySocketEvent>(message));
        }
        catch
        {
            return Task.FromResult((EasySocketEvent?)null);
        }
    }

    public sealed override Task<string?> ExtractMessage(EasySocketEvent @event, string message)
    {
        return Task.FromResult((string?)@event.Message);
    }

    public sealed override Task<string?> BindEvent(string @event, string message)
    {
        try
        {
            return Task.FromResult((string?)JsonSerializer.Serialize(new EasySocketEvent { Event = @event, Message = message }));
        }
        catch
        {
            return Task.FromResult((string?)null);
        }
    }
}