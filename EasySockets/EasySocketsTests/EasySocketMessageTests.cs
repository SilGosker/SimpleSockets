namespace EasySockets;

public class EasySocketMessageTests
{
    [Theory]
    [InlineData("Hello, World!")]
    [InlineData("Goodbye, World!")]
    public void EasySocketMessage_CanBeCreated(string message)
    {
        // Arrange & Act
        var easySocketMessage = new EasySocketMessage(message);

        // Assert
        Assert.Equal(message, easySocketMessage.Message);
    }

    [Theory]
    [InlineData("Hello, World!")]
    [InlineData("Goodbye, World!")]
    public void EasySocketMessage_CanSetMessage(string message)
    {
        // Arrange
        var easySocketMessage = new EasySocketMessage(string.Empty);

        // Act
        easySocketMessage.Message = message;

        // Assert
        Assert.Equal(message, easySocketMessage.Message);
    }

    [Fact]
    public void EasySocketMessage_CanSetCancellationToken()
    {
        var easySocketMessage = new EasySocketMessage(string.Empty);
        var cancellationToken = new CancellationToken();
        easySocketMessage.CancellationToken = cancellationToken;
        Assert.Equal(cancellationToken, easySocketMessage.CancellationToken);
    }
}