namespace EasySockets.Authentication;

public class EasySocketsAuthenticationResultTests
{
    [Fact]
    public void Constructor_WhenOnlyAuthenticated_ShouldSetIsAuthenticated()
    {
        var isAuthenticated = true;

        var result = new EasySocketAuthenticationResult(isAuthenticated);

        Assert.Equal(isAuthenticated, result.IsAuthenticated);
        Assert.Null(result.RoomId);
        Assert.Null(result.ClientId);
    }

    [Fact]
    public void Constructor_WhenAuthenticatedAndRoomId_ShouldSetBothProperties()
    {
        var isAuthenticated = true;
        var roomId = "roomId";

        var result = new EasySocketAuthenticationResult(isAuthenticated, roomId);

        Assert.Equal(isAuthenticated, result.IsAuthenticated);
        Assert.Equal(roomId, result.RoomId);
        Assert.Null(result.ClientId);
    }

    [Fact]
    public void Constructor_WhenAuthenticatedRoomIdAndClientId_ShouldSetAllProperties()
    {
        var isAuthenticated = true;
        var roomId = "roomId";
        var clientId = "clientId";

        var result = new EasySocketAuthenticationResult(isAuthenticated, roomId, clientId);

        Assert.Equal(isAuthenticated, result.IsAuthenticated);
        Assert.Equal(roomId, result.RoomId);
        Assert.Equal(clientId, result.ClientId);
    }

    [Fact]
    public void ImplicitConversion_FromBoolean_ShouldSetIsAuthenticated()
    {
        var isAuthenticated = true;

        EasySocketAuthenticationResult result = isAuthenticated;

        Assert.Equal(isAuthenticated, result.IsAuthenticated);
        Assert.Null(result.RoomId);
        Assert.Null(result.ClientId);
    }
}