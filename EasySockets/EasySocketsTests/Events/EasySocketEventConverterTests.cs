using System.Text.Json;

namespace EasySockets.Events;

public class EasySocketEventConverterTests
{
    [Theory]
    [InlineData("{\"Event\":\"TestEvent\",\"Message\":\"TestMessage\"}", false)]
    [InlineData("{\"event\":\"TestEvent\",\"message\":\"TestMessage\"}", true)]
    public void Serialize_WithCasingPolicy_ShouldMatchCasing(string expected, bool useCamelCasing)
    {
        var easySocketEvent = new EasySocketEvent("TestEvent", "TestMessage");

        string json = JsonSerializer.Serialize(easySocketEvent, new JsonSerializerOptions()
        {
            Converters = { new EasySocketEventConverter() },
            PropertyNamingPolicy = useCamelCasing ? JsonNamingPolicy.CamelCase : null
        });

        Assert.Equal(expected, json);
    }

    [Fact]
    public void Serialize_WithEasySocketEventConverterOptions_ShouldMatchExactForm()
    {
        var easySocketEvent = new EasySocketEvent("TestEvent", "TestMessage");

        string json =
            JsonSerializer.Serialize(easySocketEvent, EasySocketEventConverter.EasySocketEventSerializerOptions);

        Assert.Equal("{\"Event\":\"TestEvent\",\"Message\":\"TestMessage\"}", json);
    }

    [Theory]
    [InlineData("{\"event\":\"TestEvent\",\"message\":\"TestMessage\"}", true)]
    [InlineData("{\"Event\":\"TestEvent\",\"Message\":\"TestMessage\"}", false)]
    public void Deserialize_WithCasingPolicy_ShouldMatchCasing(string json, bool useCamelCasing)
    {
        var easySocketEvent =
            JsonSerializer.Deserialize<EasySocketEvent>(json,
                new JsonSerializerOptions()
                {
                    Converters = { new EasySocketEventConverter() },
                    PropertyNamingPolicy = useCamelCasing ? JsonNamingPolicy.CamelCase : null
                });

        Assert.NotNull(easySocketEvent);
        Assert.Equal("TestEvent", easySocketEvent.Event);
        Assert.Equal("TestMessage", easySocketEvent.Message);
    }

    [Fact]
    public void Deserialize_WithEasySocketEventConverterOptions_ShouldMatchExactForm()
    {
        string json = "{\"Event\":\"TestEvent\",\"Message\":\"TestMessage\"}";

        var easySocketEvent =
            JsonSerializer.Deserialize<EasySocketEvent>(json,
                EasySocketEventConverter.EasySocketEventSerializerOptions);

        Assert.NotNull(easySocketEvent);
        Assert.Equal("TestEvent", easySocketEvent.Event);
        Assert.Equal("TestMessage", easySocketEvent.Message);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"Message\":\"TestMessage\"}")]
    [InlineData("{\"Event\":\"TestEvent\"}")]
    [InlineData("{\"ExtraField\":\"Value\",\"Event\":\"TestEvent\",\"Message\":\"TestMessage\"}")]
    public void Deserialize_InvalidJson_ShouldThrowJsonException(string json)
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<EasySocketEvent>(json,
                EasySocketEventConverter.EasySocketEventSerializerOptions));
    }
}