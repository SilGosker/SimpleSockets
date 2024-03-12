using EasySockets.Mock;

namespace EasySockets.Services;

public class EasySocketRoomTests
{
    [Theory]
    [InlineData("test")]
    [InlineData("test2")]
    [InlineData("test3")]
    public void Constructor_ShouldCreateRoom(string id)
    {
        // Arrange
        var room = new EasySocketRoom(id, new MockEasySocket());

        // Assert
        Assert.NotNull(room.Sockets);
        Assert.Equal(id, room.Id);
        Assert.Single(room.Sockets);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("test", null)]
    public void Constructor_WhenGivenNullValues_ShouldThrowArgumentNullException(string? id, IEasySocket? easySocket)
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ObjectCreationAsStatement
            new EasySocketRoom(id!, easySocket!);
        });
    }
}