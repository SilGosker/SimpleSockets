using System.Net.WebSockets;
using System.Text.Json;
using SimpleSockets.Builder;
using SimpleSockets.DataModels;

namespace SimpleSockets;

public abstract class EventSocket : EventSocket<SimpleSocketEvent>
{
    protected EventSocket(WebSocket webSocket, SimpleSocketOptions options) : base(webSocket, options)
    {
    }

    public sealed override Task<SimpleSocketEvent?> ExtractEvent(string message)
    {
        try
        {
            return Task.FromResult(JsonSerializer.Deserialize<SimpleSocketEvent>(message));
        }
        catch
        {
            return Task.FromResult((SimpleSocketEvent?)null);
        }
    }

    public sealed override Task<string?> ExtractMessage(SimpleSocketEvent @event, string message)
    {
        return Task.FromResult((string?)@event.Message);
    }

    public sealed override Task<string?> BindEvent(string @event, string message)
    {
        try
        {
            return Task.FromResult((string?)JsonSerializer.Serialize(new SimpleSocketEvent { Event = @event, Message = message }));
        }
        catch
        {
            return Task.FromResult((string?)null);
        }
    }
}