namespace EasySockets.Builder;

public class EasySocketGlobalOptionsTests
{
    private const string TestRoomId = "test";
    private const string TestClientId = "test";

    [Fact]
    public void Constructor_InitializesAllProperties_WithNonNullValues()
    {
        var options = new EasySocketGlobalOptions();

        Assert.NotNull(options.WebSocketOptions);
        Assert.NotNull(options.GetDefaultRoomId);
        Assert.NotNull(options.GetDefaultClientId);
    }

    [Fact]
    public void GetDefaultRoomId_WhenConstructed_ReturnsDefaultRoomId()
    {
        var options = new EasySocketGlobalOptions();

        Assert.Equal("__0", options.GetDefaultRoomId(null!));
    }

    [Fact]
    public void GetDefaultClientId_WhenConstructed_ReturnsDefaultClientId()
    {
        var options = new EasySocketGlobalOptions();

        var clientId1 = options.GetDefaultClientId(null!);
        var clientId2 = options.GetDefaultClientId(null!);

        Assert.NotEmpty(clientId1);
        Assert.NotEmpty(clientId2);
        Assert.True(Guid.TryParse(clientId1, out _));
        Assert.True(Guid.TryParse(clientId2, out _));
        Assert.NotEqual(clientId1, clientId2);
    }

    [Fact]
    public void WebSocketOptions_WhenConstructed_ReturnsNewWebSocketOptions()
    {
        var options = new EasySocketGlobalOptions();

        Assert.NotNull(options.WebSocketOptions);
    }

    [Theory]
    [InlineData("GetDefaultRoomId")]
    [InlineData("GetDefaultClientId")]
    public void GetDefaultPropertySetters_WhenInvokedWithNonNullValues_ShouldUpdateProperties(string propertyName)
    {
        var options = new EasySocketGlobalOptions();

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
        var options = new EasySocketGlobalOptions();

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