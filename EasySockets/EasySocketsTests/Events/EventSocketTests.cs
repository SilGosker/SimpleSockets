using EasySockets.Mock;

namespace EasySockets.Events;

public class EventSocketTests
{
    [Fact]
    public void ExtractEvent_WhenMessageIsNotJson_ShouldReturnNull()
    {
        var socket = new MockEventSocket();
        var message = "Hello World!";

        var @event = socket.ExtractEvent(message);

        Assert.Null(@event);
    }

    [Theory]
    [InlineData("{\"event\":\"Hello\"}")]
    [InlineData("{\"message\":\"World!\"}")]
    public void ExtractEvent_WhenFieldsAreMissing_ShouldReturnNull(string message)
    {
        var socket = new MockEventSocket();
        
        var @event = socket.ExtractEvent(message);

        Assert.Null(@event);
    }

    [Fact]
    public void ExtractEvent_WhenExtraFieldsAreAdded_ShouldReturnNull()
    {
        var socket = new MockEventSocket();
        var message = "{\"event\":\"message\",\"message\":\"Hello World!\",\"data\":\"Hello World 2.0!\"}";

        var @event = socket.ExtractEvent(message);

        Assert.Null(@event);
    }
    [Fact]
    public void ExtractEvent_WhenMessageContainsValidJson_ShouldReturnEvent()
    {
        var socket = new MockEventSocket();
        var message = "{\"event\":\"event\",\"message\":\"Hello World!\"}";

        var @event = socket.ExtractEvent(message);

        Assert.NotNull(@event);
        Assert.Equal("event", @event.Event);
        Assert.Equal("Hello World!", @event.Message);
    }

    [Fact]
    public void ExtractMessage_WhenMessageContainsMessage_ShouldReturnMessage()
    {
        var socket = new MockEventSocket();
        var input = "{\"event\":\"event\",\"message\":\"Hello World!\"}";
        var @event = socket.ExtractEvent(input);

        var message = socket.ExtractMessage(@event!, input);

        Assert.NotNull(message);
        Assert.Equal("Hello World!", message);
    }

    [Fact]
    public void BindMessage_ShouldReturnJson()
    {
        var socket = new MockEventSocket();
        var @event = "event";
        var message = "Hello World!";

        var result = socket.BindEvent(@event, message);

        Assert.NotNull(result);
        Assert.Equal("{\"Event\":\"event\",\"Message\":\"Hello World!\"}", result);
    }
}