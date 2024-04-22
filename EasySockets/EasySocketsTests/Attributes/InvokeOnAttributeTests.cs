namespace EasySockets.Attributes;

public class InvokeOnAttributeTests
{
    [Theory]
    [InlineData("test")]
    [InlineData("test2")]
    public void Constructor_ShouldCreateValidInstance(string @event)
    {
        var attribute = new InvokeOnAttribute(@event);

        Assert.Equal(@event, attribute.Event);
    }

    [Fact]
    public void Constructor_WhenGivenNullEvent_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ObjectCreationAsStatement
            new InvokeOnAttribute(null!);
        });
    }
}