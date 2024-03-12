namespace EasySockets.Builder;

public class EasySocketMiddlewareOptionsTests
{
    private const string TestRoomId = "test";
    private const string TestClientId = "test";

    [Fact]
    public void Constructor_InitializesAllProperties_WithNonNullValues()
    {
        var options = new EasySocketMiddlewareOptions();

        Assert.NotNull(options.WebSocketOptions);
        Assert.NotNull(options.GetDefaultRoomId);
        Assert.NotNull(options.GetDefaultClientId);
    }

    [Theory]
    [InlineData("GetDefaultRoomId")]
    [InlineData("GetDefaultClientId")]
    public void GetDefaultPropertySetters_WhenInvokedWithNonNullValues_ShouldUpdateProperties(string propertyName)
    {
        var options = new EasySocketMiddlewareOptions();

        switch (propertyName)
        {
            case "GetDefaultRoomId":
                options.GetDefaultRoomId = _ => TestRoomId;
                Assert.Equal(TestRoomId, options.GetDefaultRoomId(null!));
                break;
            case "GetDefaultClientId":
                options.GetDefaultClientId = _ => TestClientId;
                Assert.Equal(TestClientId, options.GetDefaultClientId(null!));
                break;
        }
    }

    [Theory]
    [InlineData("GetDefaultRoomId")]
    [InlineData("GetDefaultClientId")]
    [InlineData("WebSocketOptions")]
    public void PropertySetters_WhenSetToNull_ShouldThrowArgumentNullException(string propertyName)
    {
        var options = new EasySocketMiddlewareOptions();

        switch (propertyName)
        {
            case "GetDefaultRoomId":
                Assert.Throws<ArgumentNullException>(() => options.GetDefaultRoomId = null!);
                break;
            case "GetDefaultClientId":
                Assert.Throws<ArgumentNullException>(() => options.GetDefaultClientId = null!);
                break;
            case "WebSocketOptions":
                Assert.Throws<ArgumentNullException>(() => options.WebSocketOptions = null!);
                break;
        }
    }
}