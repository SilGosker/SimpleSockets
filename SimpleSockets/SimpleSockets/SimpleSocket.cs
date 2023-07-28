using Microsoft.Extensions.Options;
using SimpleSockets.Options;
using System.Diagnostics;
using System.Net.WebSockets;

namespace SimpleSockets;

[DebuggerDisplay("{RoomId}.{UserId} = {_webSocket.State}")]
public abstract class SimpleSocket : SimpleSocket<string>
{
    protected internal SimpleSocket(WebSocket webSocket, SimpleSocketOptions options) : base(webSocket, options)
    {
    }
    
    public sealed override string RegisterEvent(string message)
    {
        return message[..message.IndexOf(':')];
    }
    
    public sealed override string CreateEvent(string @event, string message)
    {
        return $"{@event}:{message}";
    }

    /// <summary>
    /// This method is called when a message is received from the websocket.
    /// </summary>
    /// <param name="message">The received message</param>
    /// <returns>A task representing the operation</returns>
    public abstract override Task OnMessage(string message);
}