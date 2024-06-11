using System.Text;
using EasySockets.Extensions;

namespace EasySockets.Builder;

public class ReadonlyEasySocketOptionsTests
{
    [Fact]
    public void Initialization_ShouldSetValues()
    {
        // Arrange
        var options = new ReadonlyEasySocketOptions
        {
            ClosingStatusDescription = "Closing",
            Encoding = Encoding.UTF8,
            ReceiveBufferSize = 100_000,
            SendBufferSize = 100_000,
        };

        // Act
        var result = options;

        // Assert
        Assert.Equal("Closing", result.ClosingStatusDescription);
        Assert.Equal(Encoding.UTF8, result.Encoding);
        Assert.Equal(100_000, result.ReceiveBufferSize);
        Assert.Equal(100_000, result.SendBufferSize);
    }
}