using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using EasySockets.Enums;
using EasySockets.Mock;
using EasySockets.Services.Caching;
using Moq;

namespace EasySockets.Events;

public class GenericEventSocketTests
{
    [Fact]
    public void SendToClientAsync_WithInvalidEvent_ShouldNotCallSendToClientAsync()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var message = "Hello, World!";
        var @event = "InvalidEvent";

        // Act
        var result = eventSocket.Object.SendToClientAsync(@event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void SendToClientAsync_WithValidEvent_ShouldCallSendToClientAsync()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var mockWebsocket = new Mock<WebSocket>();
        mockWebsocket.Setup(x => x.State).Returns(WebSocketState.Open);
        ((IInternalEasySocket)eventSocket.Object).WebSocket = mockWebsocket.Object;

        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(string.Empty);

        var message = "Hello, World!";
        var @event = "MockEvent";

        // Act
        var result = eventSocket.Object.SendToClientAsync(@event, message);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Task.CompletedTask, result);
    }

    [Fact]
    public void SendToClientAsync_WithNullEvent_ShouldNotCallSendToClientAsync()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var message = "Hello, World!";
        string @event = null!;

        // Act
        var result = eventSocket.Object.SendToClientAsync(@event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void SendToClientAsyncWithType_WithInvalidEvent_ShouldNotCallSendToClientAsync()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string?)null);

        var message = "Hello, World!";
        var @event = new MockEvent { Event = "InvalidEvent" };

        // Act
        var result = eventSocket.Object.SendToClientAsync(@event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void SendToClientAsyncWithType_WithValidEvent_ShouldCallSendToClientAsync()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var mockWebsocket = new Mock<WebSocket>();
        mockWebsocket.Setup(x => x.State).Returns(WebSocketState.Open);
        ((IInternalEasySocket)eventSocket.Object).WebSocket = mockWebsocket.Object;

        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>())).Returns("MockEvent");

        var message = "Hello, World!";
        var @event = new MockEvent { Event = "MockEvent" };

        // Act
        var result = eventSocket.Object.SendToClientAsync(@event, message);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Task.CompletedTask, result);
    }

    [Fact]
    public void SendToClientAsyncWithType_WithNullEvent_ShouldNotCallSendToClientAsync()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string?)null);

        var message = "Hello, World!";
        var @event = new MockEvent { Event = null! };

        // Act
        var result = eventSocket.Object.SendToClientAsync(@event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void Broadcast_WithInvalidEvent_ShouldNotCallBroadcast()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string?)null);

        var message = "Hello, World!";
        var @event = "InvalidEvent";

        // Act
        var result = eventSocket.Object.Broadcast(BroadCastFilter.Everyone, @event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void Broadcast_WithValidEvent_ShouldCallBroadcast()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();

        ((IInternalEasySocket)eventSocket.Object).Emit =
            (_, _, _) => Task.Delay(1); // prevent returning Task.CompletedTask

        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("MockEvent");

        var message = "Hello, World!";
        var @event = "MockEvent";

        // Act
        var result = eventSocket.Object.Broadcast(BroadCastFilter.Everyone, @event, message);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Task.CompletedTask, result);
    }

    [Fact]
    public void BroadcastWithType_WithInvalidEvent_ShouldNotCallBroadcast()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string?)null);

        var message = "Hello, World!";
        var @event = new MockEvent { Event = "InvalidEvent" };

        // Act
        var result = eventSocket.Object.Broadcast(BroadCastFilter.Everyone, @event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void BroadcastWithType_WithValidEvent_ShouldCallBroadcast()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        eventSocket.Setup(x => x.BindEvent(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("MockEvent");

        ((IInternalEasySocket)eventSocket.Object).Emit =
            (_, _, _) => Task.Delay(1); // prevent returning Task.CompletedTask

        var message = "Hello, World!";
        var @event = new MockEvent { Event = "MockEvent" };

        // Act
        var result = eventSocket.Object.Broadcast(BroadCastFilter.Everyone, @event, message);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Task.CompletedTask, result);
    }

    [Fact]
    public void Broadcast_WithNullEvent_ShouldNotCallBroadcast()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var message = "Hello, World!";
        string @event = null!;

        // Act
        var result = eventSocket.Object.Broadcast(BroadCastFilter.Everyone, @event, message);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void OnMessage_ShouldExtractEvent()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var message = "{\"event\":\"MockEvent\",\"data\":\"Hello, World!\"}";

        // Act
        eventSocket.Object.OnMessage(message);

        // Assert
        eventSocket.Verify(x => x.ExtractEvent(message), Times.Once);
    }

    [Fact]
    public async Task OnMessage_WhenMessageIsInvalid_ShouldCallOnFailedEventBinding()
    {
        // Arrange
        var eventSocket = new Mock<EventSocket<MockEvent>>();
        var message = "{\"event\":\"InvalidEvent\",\"data\":\"Hello, World!\"}";

        // Act
        await eventSocket.Object.OnMessage(message);

        // Assert
        eventSocket.Verify(x => x.OnFailedEventBinding(message), Times.Once);
    }

    [Fact]
    public async Task OnMessage_WhenMessageIsValid_ShouldNotCallOnFailedEventBinding()
    {
        // Arrange
        var task = Task.Delay(1);
        var eventSocket = new Mock<MockEventSocket>();
        eventSocket.Setup(e => e.OnEventAsync(It.IsAny<string>())).Returns(task);
        eventSocket.Setup(e => e.ExtractEvent(It.IsAny<string>())).Returns(new MockEvent()
        {
            Event = "Not_MockEvent"
        });
        ((IInternalEventSocket)eventSocket.Object).Events = new[]
        {
            new EventSocketEventInfo(
                eventSocket.Object.GetType().GetMethod(nameof(MockEventSocket.OnEventAsync), BindingFlags.Instance | BindingFlags.Public)!,
                new[] { "MockEvent" })
        };


        // Act
        await eventSocket.Object.OnMessage(It.IsAny<string>());

        // Assert
        eventSocket.Verify(x => x.OnFailedEventBinding(It.IsAny<string>()), Times.Never);
        eventSocket.Verify(x => x.OnEventAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void OnMessage_WhenNoEventIsFound_ShouldNotInvokeEvents()
    {
        var eventSocket = new Mock<MockEventSocket>();
        eventSocket.Setup(e => e.ExtractEvent(It.IsAny<string>())).Returns(new MockEvent()
        {
            Event = "Not_MockEvent"
        });
        ((IInternalEventSocket)eventSocket.Object).Events = new[]
        {
            new EventSocketEventInfo(
                eventSocket.Object.GetType().GetMethod(nameof(MockEventSocket.OnEventAsync), BindingFlags.Instance | BindingFlags.Public)!,
                new[] { "MockEvent" })
        };

        var result = eventSocket.Object.OnMessage(It.IsAny<string>());

        Assert.Equal(Task.CompletedTask, result);
    }

    [Fact]
    public void OnMessage_WhenEventIsFound_ShouldInvokeEvent()
    {
        var task = Task.Delay(1);
        var eventSocket = new Mock<MockEventSocket>();
        eventSocket.Setup(e => e.OnEventAsync(It.IsAny<string>())).Returns(task);
        eventSocket.Setup(e => e.ExtractEvent(It.IsAny<string>())).Returns(new MockEvent()
        {
            Event = "MockEvent"
        });
        ((IInternalEventSocket)eventSocket.Object).Events = new[]
        {
            new EventSocketEventInfo(
                eventSocket.Object.GetType().GetMethod(nameof(MockEventSocket.OnEventAsync), BindingFlags.Instance | BindingFlags.Public)!,
                new[] { "MockEvent" })
        };

        var result = eventSocket.Object.OnMessage(string.Empty);

        Assert.Equal(task, result);
    }
}