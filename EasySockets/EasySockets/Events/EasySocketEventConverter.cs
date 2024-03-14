using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasySockets.Events;

public sealed class EasySocketEventConverter : JsonConverter<EasySocketEvent>
{
    public static readonly JsonSerializerOptions EasySocketEventSerializerOptions = new()
    {
        Converters = { new EasySocketEventConverter() },
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    public override EasySocketEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        string? eventValue = null;
        string? messageValue = null;
        bool eventPropertyExists = false;
        bool messagePropertyExists = false;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "Event" when options.PropertyNamingPolicy != JsonNamingPolicy.CamelCase || options.PropertyNameCaseInsensitive:
                    case "event" when options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase || options.PropertyNameCaseInsensitive:
                        eventValue = reader.GetString();
                        eventPropertyExists = true;
                        break;
                    case "message" when options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase || options.PropertyNameCaseInsensitive:
                    case "Message" when options.PropertyNamingPolicy != JsonNamingPolicy.CamelCase || options.PropertyNameCaseInsensitive:
                        messageValue = reader.GetString();
                        messagePropertyExists = true;
                        break;
                    default:
                        throw new JsonException();
                }
            }
        }

        if (eventPropertyExists && messagePropertyExists && eventValue != null && messageValue != null)
        {
            return new EasySocketEvent(eventValue, messageValue);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, EasySocketEvent value, JsonSerializerOptions options)
    {
        if (options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase)
        {
            writer.WriteStartObject();
            writer.WriteString("event", value.Event);
            writer.WriteString("message", value.Message);
            writer.WriteEndObject();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("Event", value.Event);
        writer.WriteString("Message", value.Message);
        writer.WriteEndObject();
    }
}