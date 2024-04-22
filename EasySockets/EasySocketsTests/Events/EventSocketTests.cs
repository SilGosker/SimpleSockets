using EasySockets.Mock;
using Moq;

namespace EasySockets.Events;

public class EventSocketTests
{
    [Fact]
    public void ExtractEvent_WhenMessageIsNotJson_ShouldReturnNull()
    {
        var socket = new Moq.Mock<EventSocket>();
        var message = "Hello World!";

        var @event = socket.Object.ExtractEvent(message);

        Assert.Null(@event);
    }

    [Theory]
    [InlineData("{\"event\":\"Hello\"}")]
    [InlineData("{\"message\":\"World!\"}")]
    public void ExtractEvent_WhenFieldsAreMissing_ShouldReturnNull(string message)
    {
        var socket = new Mock<EventSocket>();
        
        var @event = socket.Object.ExtractEvent(message);

        Assert.Null(@event);
    }

    [Fact]
    public void ExtractEvent_WhenExtraFieldsAreAdded_ShouldReturnNull()
    {
        var socket = new Mock<EventSocket>();

        var message = "{\"event\":\"message\",\"message\":\"Hello World!\",\"data\":\"Hello World 2.0!\"}";

        var @event = socket.Object.ExtractEvent(message);

        Assert.Null(@event);
    }

    [Fact]
    public void ExtractEvent_WhenMessageContainsValidJson_ShouldReturnEvent()
    {
        var socket = new Mock<EventSocket>();

        var message = "{\"event\":\"event\",\"message\":\"Hello World!\"}";

        var @event = socket.Object.ExtractEvent(message);

        Assert.NotNull(@event);
        Assert.Equal("event", @event.Event);
        Assert.Equal("Hello World!", @event.Message);
    }

    [Fact]
    public void BindMessage_ShouldReturnJson()
    {
        var socket = new Mock<EventSocket>();
        
        var @event = "event";
        var message = "Hello World!";

        var result = socket.Object.BindEvent(@event, message);

        Assert.NotNull(result);
        Assert.Equal("{\"Event\":\"event\",\"Message\":\"Hello World!\"}", result);
    }
}