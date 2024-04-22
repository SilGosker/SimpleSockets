using EasySockets.Mock;
using EasySockets.Services.Caching;

namespace EasySockets.Builder;

public class EasySocketBuilderTests
{
	private const string Url = "/test";

	[Fact]
	public void AddEasySocket_ShouldThrowNullException_IfParameterIsNull()
	{
        var builder = new EasySocketBuilder(new EasySocketTypeHolder());

		Assert.Throws<ArgumentNullException>(() =>
        {
            builder.AddEasySocket<MockEasySocket>(null!);
        });
	}

	[Fact]
	public void AddEasySocket_ShouldReturnItself()
	{
		var builder = new EasySocketBuilder(new EasySocketTypeHolder());

		var builder2 = builder.AddEasySocket<MockEasySocket>(Url);

		Assert.Equal(builder, builder2);
	}

	[Fact]
	public void AddEasySocket_ShouldThrowInvalidOperation_IfUrlIsRegistered()
	{
		var builder = new EasySocketBuilder(new EasySocketTypeHolder());

		builder.AddEasySocket<MockEasySocket>(Url);

		Assert.Throws<InvalidOperationException>(() =>
		{
			builder.AddEasySocket<MockEasySocket>(Url);
		});
	}
}