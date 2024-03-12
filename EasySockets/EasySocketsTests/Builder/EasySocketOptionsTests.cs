using System.Text;
// ReSharper disable UseObjectOrCollectionInitializer : Unnecessary in this case, as it divides the arrange/act/assert sections of the tests

namespace EasySockets.Builder;

public class EasySocketOptionsTests
{
	[Fact]
	public void ChunkSizeSetter_IfLowerThanOne_ShouldThrowArgumentOutOfRangeException()
	{
		// Arrange
		var options = new EasySocketOptions();

		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => options.ChunkSize = -1);
        Assert.Throws<ArgumentOutOfRangeException>(() => options.ChunkSize = 0);
    }

    [Fact]
	public void PropertySetters_WhenSetToNull_ShouldThrowArgumentNullException()
	{
		// Arrange
		var options = new EasySocketOptions();

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => options.Encoding = null!);
        Assert.Throws<ArgumentNullException>(() => options.ClosingStatusDescription = null!);
    }

	[Fact]
	public void ChunkSizeSetter_WhenSetToValidValue_ShouldUpdateValue()
	{
		// Arrange
		var options = new EasySocketOptions();

		// Act
		options.ChunkSize = 100;

		// Assert
        Assert.Equal(100, options.ChunkSize);
	}

	[Fact]
	public void EncodingSetter_WhenSetToValidValue_ShouldUpdateValue()
	{
        // Arrange
		var options = new EasySocketOptions();

		// Act
		options.Encoding = Encoding.ASCII;

		// Assert
		Assert.Equal(Encoding.ASCII, options.Encoding);
	}

	[Fact]
	public void ClosingStatusDescriptionSetter_WhenSetToValidValue_ShouldUpdateValue()
	{
		// Arrange
		var options = new EasySocketOptions();

		// Act
        options.ClosingStatusDescription = "Test";

		// Assert
		Assert.Equal("Test", options.ClosingStatusDescription);
	}
}