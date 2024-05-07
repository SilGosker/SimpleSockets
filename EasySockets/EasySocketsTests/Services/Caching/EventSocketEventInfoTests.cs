using System.Reflection;
using EasySockets.Events;
using EasySockets.Mock;
using Moq;

namespace EasySockets.Services.Caching;

public class EventSocketEventInfoTests
{
    [Fact]
    public void Contains_WhenEventNameIsInEventNames_ReturnsTrue()
    {
        // Arrange
        var method = typeof(EventSocketEventInfo).GetMethod(nameof(EventSocketEventInfo.Contains), BindingFlags.Instance | BindingFlags.NonPublic);
        var eventNames = new string[] { "event1", "event2" };
        var eventSocketEventInfo = new EventSocketEventInfo(method!, eventNames);
        var eventName = "event1";

        // Act
        var result = eventSocketEventInfo.Contains(eventName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_WhenEventNameIsNotInEventNames_ReturnsFalse()
    {
        // Arrange
        var method = typeof(EventSocketEventInfo).GetMethod(nameof(EventSocketEventInfo.Contains), BindingFlags.Instance | BindingFlags.NonPublic);
        var eventNames = new string[] { "event1", "event2" };
        var eventSocketEventInfo = new EventSocketEventInfo(method!, eventNames);
        var eventName = "event3";

        // Act
        var result = eventSocketEventInfo.Contains(eventName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void InvokeAsync_WhenResultIsTask_ReturnsTask()
    {
        // Arrange
        var task = Task.Delay(1);
        var instance = new Mock<MockEventSocket>();
        instance.Setup(x => x.OnEventAsync(It.IsAny<string>())).Returns(task);

        var method = instance.Object.GetType().GetMethod(nameof(MockEventSocket.OnEventAsync), BindingFlags.Instance | BindingFlags.Public);
        var eventNames = new string[] { "event1", "event2" };
        var eventSocketEventInfo = new EventSocketEventInfo(method!, eventNames);
        var message = new MockEvent();

        // Act
        var result = eventSocketEventInfo.InvokeAsync(instance.Object, message, message.Event);

        // Assert
        Assert.Equal(task, result);
    }

    [Fact]
    public void InvokeAsync_WhenResultIsNotTask_ReturnsCompletedTask()
    {
        // Arrange
        var instance = new Mock<MockEventSocket>();
        var method = instance.Object.GetType().GetMethod(nameof(MockEventSocket.OnEvent), BindingFlags.Instance | BindingFlags.Public);
        var eventNames = new string[] { "event1", "event2" };
        var eventSocketEventInfo = new EventSocketEventInfo(method!, eventNames);
        var message = new MockEvent();

        // Act
        var result = eventSocketEventInfo.InvokeAsync(instance.Object, message, message.Event);

        // Assert
        Assert.Equal(Task.CompletedTask, result);
    }
}