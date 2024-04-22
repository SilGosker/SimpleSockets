using System.Xml.Serialization;
using CustomEventTypes.Events;
using EasySockets.Events;

namespace CustomEventTypes.Sockets;

public class XmlEventSocket : EventSocket<XmlEvent>
{
    private static readonly XmlSerializer _serializer = new(typeof(XmlEvent));

    public override XmlEvent? ExtractEvent(string message)
    {
        try
        {
            using var reader = new StringReader(message);
            return (XmlEvent?)_serializer.Deserialize(reader);
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
        
        _serializer.Serialize(writer, xmlEvent);
        
        return writer.ToString();
    }
}