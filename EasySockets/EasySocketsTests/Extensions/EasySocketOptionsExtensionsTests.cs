using System.Text;
using EasySockets.Builder;

namespace EasySockets.Extensions;

public class EasySocketOptionsExtensionsTests
{
    [Fact]
    public void AsReadonly_ReturnsReadonlyEasySocketOptions()
    {
        // Arrange
        var options = new EasySocketOptions
        {
            ClosingStatusDescription = "Closing",
            Encoding = Encoding.UTF8,
            ReceiveBufferSize = 100_000,
            SendBufferSize = 100_000,
        };

        // Act
        var result = options.AsReadonly();

        // Assert
        Assert.Equal("Closing", result.ClosingStatusDescription);
        Assert.Equal(Encoding.UTF8, result.Encoding);
        Assert.Equal(100_000, result.ReceiveBufferSize);
        Assert.Equal(100_000, result.SendBufferSize);
    }
}