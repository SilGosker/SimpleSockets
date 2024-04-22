using Moq;

#pragma warning disable CS4014

namespace EasySockets.Services;

public class EasySocketServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AddSocket_WhenSocketIsConnected_AddsSocketToNewRoom(bool isConnected)
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(isConnected ? Task.CompletedTask : Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(isConnected);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        var service = new EasySocketService();

        // Act
        var task = service.AddSocket(mockSocket.Object);

        // Assert
        Assert.NotNull(task);
        await task;
    }

    [Theory]
    [InlineData("roomId", "roomId")]
    [InlineData("roomId1", "roomId2")]
    public void Count_WhenRoomIdIsOmitted_CountsAllSockets(params string[] roomIds)
    {
        List<Mock<IEasySocket>> sockets = new();

        foreach (var roomId in roomIds)
        {
            var mockSocket = new Mock<IEasySocket>();
            mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
            mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
            mockSocket.Setup(s => s.IsConnected()).Returns(true);
            mockSocket.SetupGet(s => s.RoomId).Returns(roomId);
            sockets.Add(mockSocket);
        }

        // Act
        var service = new EasySocketService();
        foreach (var socket in sockets)
        {
            service.AddSocket(socket.Object);
        }

        // Assert
        Assert.Equal(roomIds.Length, service.Count());
    }

    [Theory]
    [InlineData("roomId", "roomId", "roomId1")]
    [InlineData("roomId1", "roomId2", "roomId3")]
    public void Count_WhenRoomIdIsProvided_CountsAllSocketsInRoom(params string[] roomIds)
    {
        // Arrange
        List<Mock<IEasySocket>> sockets = new();

        foreach (var roomId in roomIds)
        {
            var mockSocket = new Mock<IEasySocket>();
            mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
            mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
            mockSocket.Setup(s => s.IsConnected()).Returns(true);
            mockSocket.SetupGet(s => s.RoomId).Returns(roomId);
            sockets.Add(mockSocket);
        }

        // Act
        var service = new EasySocketService();
        foreach (var socket in sockets)
        {
            service.AddSocket(socket.Object);
        }

        // Assert
        foreach (var roomId in roomIds)
        {
            Assert.Equal(roomIds.Count(e => e == roomId), service.Count(roomId));
        }
    }

    [Fact]
    public void Any_WhenRoomIdIsOmitted_ReturnsTrueIfAnySocketsAreConnected()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");

        var service = new EasySocketService();
        var disconnectedValue = service.Any();
        // Act
        service.AddSocket(mockSocket.Object);
        var connectedValue = service.Any();

        // Assert
        Assert.True(connectedValue);
        Assert.False(disconnectedValue);
    }

    [Fact]
    public void Any_WhenRoomIdIsProvided_ReturnsTrueIfAnySocketsAreConnectedInRoom()
    {
        // Arrange
        string roomIdConnected = "roomId";
        string roomIdDisconnected = "roomId1";
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns(roomIdConnected);

        var service = new EasySocketService();

        // Act
        service.AddSocket(mockSocket.Object);
        var connectedValue = service.Any(roomIdConnected);
        var disconnectedValue = service.Any(roomIdDisconnected);

        // Assert
        Assert.True(connectedValue);
        Assert.False(disconnectedValue);
    }

    [Theory]
    [InlineData("roomId", "connectedClient", "connectedClient")]
    [InlineData("roomId1", "connectedClient", "disconnectedClient")]
    public void Any_WhenClientIdIsProvided_ReturnsTrueIfAnySocketsAreConnectedWithClientId(string roomId, string connectedClient, string checkClient)
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns(roomId);
        mockSocket.SetupGet(s => s.ClientId).Returns(connectedClient);
        var service = new EasySocketService();

        // Act
        service.AddSocket(mockSocket.Object);
        var connectedValue = service.Any(roomId, checkClient);

        // Assert
        Assert.Equal(connectedClient == checkClient, connectedValue);
    }

    [Fact]
    public void RemoveSocket_WhenSocketIsConnected_RemovesSocketFromRoom()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        var service = new EasySocketService();
        service.AddSocket(mockSocket.Object);

        // Act
        service.RemoveSocket(mockSocket.Object);

        // Assert
        Assert.Equal(0, service.Count());
    }

    [Fact]
    public async Task ForceLeaveAsync_WhenRoomIdIsProvided_DisposesAllSocketsInRoom()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        var service = new EasySocketService();
        service.AddSocket(mockSocket.Object);

        // Act
        await service.ForceLeaveAsync("roomId");

        // Assert
        Assert.Equal(1, service.Count());
    }

    [Fact]
    public async Task ForceLeaveAsync_WhenRoomIdAndClientIdIsProvided_DisposesAllSocketsInRoomWithClientId()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        mockSocket.SetupGet(s => s.ClientId).Returns("clientId");
        var service = new EasySocketService();
        service.AddSocket(mockSocket.Object);

        // Act
        await service.ForceLeaveAsync("roomId", "clientId");

        // Assert
        Assert.Equal(1, service.Count());
    }

    [Fact]
    public async Task ForceLeaveAsync_WhenRoomIdIsProvidedAndNoSocketsExist_DoesNotThrowException()
    {
        // Arrange
        var service = new EasySocketService();

        // Act
        await service.ForceLeaveAsync("roomId");

        // Assert
        Assert.Equal(0, service.Count());
    }

    [Fact]
    public async Task ForceLeaveAsync_WhenRoomIdAndClientIdIsProvidedAndNoSocketsExist_DoesNotThrowException()
    {
        // Arrange
        var service = new EasySocketService();

        // Act
        await service.ForceLeaveAsync("roomId", "clientId");

        // Assert
        Assert.Equal(0, service.Count());
    }

    [Fact]
    public async Task SendToRoomAsync_WhenRoomIdIsProvided_SendsMessageToAllSocketsInRoom()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        var service = new EasySocketService();
        service.AddSocket(mockSocket.Object);

        // Act
        await service.SendToRoomAsync("roomId", "message");

        // Assert
        mockSocket.Verify(s => s.SendToClientAsync("message"), Times.Once);
    }

    [Fact]
    public async Task SendToRoomAsync_WhenRoomIdIsProvidedAndNoSocketsExist_DoesNotThrowException()
    {
        // Arrange
        var service = new EasySocketService();

        // Act
        await service.SendToRoomAsync("roomId", "message");

        // Assert
        Assert.Equal(0, service.Count());
    }

    [Fact]
    public async Task SendToClientAsync_WhenRoomIdAndClientIdIsProvided_SendsMessageToSocketInRoomWithClientId()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        mockSocket.SetupGet(s => s.ClientId).Returns("clientId");
        var service = new EasySocketService();
        service.AddSocket(mockSocket.Object);

        // Act
        await service.SendToClientAsync("roomId", "clientId", "message");

        // Assert
        mockSocket.Verify(s => s.SendToClientAsync("message"), Times.Once);
    }

    [Fact]
    public async Task SendToClientAsync_WhenRoomIdAndClientIdIsProvidedAndNoSocketsExist_DoesNotThrowException()
    {
        // Arrange
        var service = new EasySocketService();

        // Act
        await service.SendToClientAsync("roomId", "clientId", "message");

        // Assert
        Assert.Equal(0, service.Count());
    }

    [Fact]
    public void GetGroups_ReturnsGroupedSocketsByRoomId()
    {
        // Arrange
        var mockSocket = new Mock<IEasySocket>();
        mockSocket.Setup(s => s.ReceiveMessagesAsync()).Returns(Task.Delay(10));
        mockSocket.Setup(s => s.SendToClientAsync(string.Empty)).Verifiable();
        mockSocket.Setup(s => s.IsConnected()).Returns(true);
        mockSocket.SetupGet(s => s.RoomId).Returns("roomId");
        var service = new EasySocketService();
        service.AddSocket(mockSocket.Object);

        // Act
        var groups = service.GetGroupings();

        // Assert
        Assert.Single(groups);
    }

    [Fact]
    public void GetGroups_WhenNoSocketsExist_ReturnsEmptyGroup()
    {
        // Arrange
        var service = new EasySocketService();

        // Act
        var groups = service.GetGroupings();

        // Assert
        Assert.Empty(groups);
    }
}