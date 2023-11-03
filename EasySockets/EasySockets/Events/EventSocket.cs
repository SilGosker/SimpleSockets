using System.Net.WebSockets;
using System.Text.Json;
using EasySockets.Builder;

namespace EasySockets.Events;

public abstract class EventSocket : EventSocket<EasySocketEvent>
{
	protected EventSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
	{
	}

	public sealed override EasySocketEvent? ExtractEvent(string message)
    {
        try
        {
            return JsonSerializer.Deserialize<EasySocketEvent>(message);
        }
        catch
        {
            return null;
        }
    }

    public sealed override string? ExtractMessage(EasySocketEvent @event, string message)
    {
        return @event.Message;
    }

    public sealed override string? BindEvent(string @event, string message)
    {
        try
        {
            return JsonSerializer.Serialize(new EasySocketEvent { Event = @event, Message = message });
        }
        catch
        {
            return null;
        }
    }


}