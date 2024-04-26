using System.Xml.Serialization;
using CustomEventTypes.Events;
using EasySockets.Enums;
using EasySockets.Events;

namespace CustomEventTypes.Sockets;

public class XmlEventSocket : EventSocket<XmlEvent>
{
    private static readonly XmlSerializer Serializer = new(typeof(XmlEvent));

    /// <param name="event">The event to broadcast</param>
    /// <inheritdoc cref="EventSocket{TEvent}.Broadcast(string,string)"/>
    public Task Broadcast(XmlEvent @event)
    {
        return Broadcast(@event.Event, @event.Message);
    }

    /// <param name="event">The event to broadcast</param>
    /// <inheritdoc cref="EventSocket{TEvent}.Broadcast(BroadCastFilter,string,string)"/>
    public Task Broadcast(BroadCastFilter filter, XmlEvent @event)
    {
        return Broadcast(filter, @event.Event, @event.Message);
    }

    public override XmlEvent? ExtractEvent(string message)
    {
        try
        {
            using var reader = new StringReader(message);
            return (XmlEvent?)Serializer.Deserialize(reader);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public override string? BindEvent(string @event, string message)
    {
        var xmlEvent = new XmlEvent
        {
            Event = @event,
            Message = message
        };

        using var writer = new StringWriter();
        
        Serializer.Serialize(writer, xmlEvent);
        
        return writer.ToString();
    }
}