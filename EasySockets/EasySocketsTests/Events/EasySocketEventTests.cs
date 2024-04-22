namespace EasySockets.Events;

public class EasySocketEventTests
{
    [Fact]
    public void Constructor_WithoutParameters_ShouldSetPropertiesToEmptyStrings()
    {
        // Act
        var easySocketEvent = new EasySocketEvent();

        // Assert
        Assert.Equal(string.Empty, easySocketEvent.Event);
        Assert.Equal(string.Empty, easySocketEvent.Message);
    }

    [Fact]
    public void Constructor_WithEvent_ShouldSetEvent()
    {
        // Arrange
        var @event = "test";

        // Act
        var easySocketEvent = new EasySocketEvent(@event);

        // Assert
        Assert.Equal(@event, easySocketEvent.Event);
        Assert.Equal(string.Empty, easySocketEvent.Message);
    }

    [Fact]
	public void Constructor_WithEventAndMessage_ShouldSetEventAndMessage()
	{
        // Arrange
		var @event = "test";
		var message = "test message";

        // Act
		var easySocketEvent = new EasySocketEvent(@event, message);

        // Assert
		Assert.Equal(@event, easySocketEvent.Event);
		Assert.Equal(message, easySocketEvent.Message);
	}

    [Fact]
	public void ImplicitCastFromString_ShouldSetEvent()
	{
        // Arrange
		var @event = "test";

        // Act
        EasySocketEvent easySocketEvent = @event;

        // Assert
		Assert.Equal(@event, easySocketEvent.Event);
		Assert.Equal(string.Empty, easySocketEvent.Message);
	}

    [Fact]
    public void GetEvent_WhenEventIsSet_ShouldGetEvent()
    {
        // Arrange
        var @event = "test";
        var easySocketEvent = new EasySocketEvent(@event);

        // Act
        var result = easySocketEvent.Event;

        // Assert
        Assert.Equal(@event, result);
    }

    [Fact]
    public void Properties_WhenSet_ShouldUpdateValues()
    {
        // Arrange
        var @event = "test event";
        var message = "test message";
        // ReSharper disable once UseObjectOrCollectionInitializer
        EasySocketEvent easySocketEvent = new();

        // Act
        easySocketEvent.Event = @event;
        easySocketEvent.Message = message;

        // Assert
        Assert.Equal(@event, easySocketEvent.Event);
        Assert.Equal(message, easySocketEvent.Message);
    }
}