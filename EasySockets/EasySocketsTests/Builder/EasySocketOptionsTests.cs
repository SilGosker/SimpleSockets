// ReSharper disable UseObjectOrCollectionInitializer : Unnecessary in this case, as it divides the arrange/act/assert sections of the tests

#pragma warning disable IDE0017

using System.Text;

namespace EasySockets.Builder;

public class EasySocketOptionsTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void BufferSizeSetter_IfLowerThanOne_ShouldThrowArgumentOutOfRangeException(int chunkSize)
    {
        // Arrange
        var options = new EasySocketOptions();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.BufferSize = chunkSize);
        Assert.Throws<ArgumentOutOfRangeException>(() => options.BufferSize = chunkSize);
    }

    [Fact]
    public void PropertySetter_WhenSetToNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        EasySocketOptions options = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => options.Encoding = null!);
        Assert.Throws<ArgumentNullException>(() => options.ClosingStatusDescription = null!);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void BufferSizeSetter_WhenSetToValidValue_ShouldUpdateValue(int chunkSize)
    {
        // Arrange
        EasySocketOptions options = new();

        // Act
        options.BufferSize = chunkSize;

        // Assert
        Assert.Equal(chunkSize, options.BufferSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReceiveBufferSizeSetter_IfLowerThanOne_ShouldThrowArgumentOutOfRangeException(int value)
    {
        // Arrange
        var options = new EasySocketOptions();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.ReceiveBufferSize = value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SendBufferSizeSetter_IfLowerThanOne_ShouldThrowArgumentOutOfRangeException(int value)
    {
        // Arrange
        var options = new EasySocketOptions();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => options.SendBufferSize = value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void ReceiveBufferSizeSetter_WhenSetToValidValue_ShouldUpdateValue(int value)
    {
        // Arrange
        EasySocketOptions options = new();

        // Act
        options.ReceiveBufferSize = value;

        // Assert
        Assert.Equal(value, options.ReceiveBufferSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(int.MaxValue)]
    public void SendBufferSizeSetter_WhenSetToValidValue_ShouldUpdateValue(int value)
    {
        // Arrange
        EasySocketOptions options = new();

        // Act
        options.SendBufferSize = value;

        // Assert
        Assert.Equal(value, options.SendBufferSize);
    }

    [Fact]
    public void EncodingSetter_WhenSetToValidValue_ShouldUpdateValue()
    {
        // Arrange
        EasySocketOptions options = new();

        // Act
        options.Encoding = Encoding.ASCII;

        // Assert
        Assert.Equal(Encoding.ASCII, options.Encoding);
    }

    [Fact]
    public void ClosingStatusDescriptionSetter_WhenSetToValidValue_ShouldUpdateValue()
    {
        // Arrange
        EasySocketOptions options = new();

        // Act
        options.ClosingStatusDescription = "Test";

        // Assert
        Assert.Equal("Test", options.ClosingStatusDescription);
    }
}