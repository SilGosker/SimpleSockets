using System.Net.WebSockets;
using EasySockets.Mock;
using Moq;

namespace EasySockets;

public class EasySocketTests
{
    [Theory]
    [InlineData(WebSocketState.Aborted)]
    [InlineData(WebSocketState.Open)]
    [InlineData(WebSocketState.Closed)]
    [InlineData(WebSocketState.CloseSent)]
    [InlineData(WebSocketState.CloseReceived)]
    public void IsConnected_WhenWebsocketStateIsOpen_ReturnsTrue(WebSocketState state)
    {
        var socket = new MockEasySocket();
        var webSocket = new Mock<WebSocket>();
        webSocket.SetupGet(x => x.State).Returns(state);

        ((IInternalEasySocket)socket).WebSocket = webSocket.Object;
        Assert.Equal(state == WebSocketState.Open, socket.IsConnected());
    }

    [Fact]
    public void SetRoomId_SetsRoomId()
    {
        string roomId = "test";
        var socket = new MockEasySocket();
        ((IInternalEasySocket)socket).RoomId = roomId;
        Assert.Equal(roomId, socket.RoomId);
    }

    [Fact]
    public void SetClientId_SetsClientId()
    {
        string clientId = "test";
        var socket = new MockEasySocket();
        ((IInternalEasySocket)socket).ClientId = clientId;
        Assert.Equal(clientId, socket.ClientId);
    }

    [Fact]
    public void SetWebSocket_SetsWebSocket()
    {
        var webSocket = new Mock<WebSocket>();
        var socket = new MockEasySocket();
        ((IInternalEasySocket)socket).WebSocket = webSocket.Object;

        // Assert
        socket.IsConnected(); // throws exception if webSocket is null
    }

    [Fact]
    public async Task CloseAsync_CallsOnDisconnect()
    {
        var socket = new Mock<EasySocket>();
        var webSocket = new Mock<WebSocket>();
        ((IInternalEasySocket)socket.Object).WebSocket = webSocket.Object;
        ((IInternalEasySocket)socket.Object).Options = new ();

        // Act
        await socket.Object.CloseAsync();

        // Assert
        socket.Verify(x => x.OnDisconnect(), Times.Once);
    }
}