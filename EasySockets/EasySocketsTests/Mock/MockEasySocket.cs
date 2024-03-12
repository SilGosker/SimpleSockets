namespace EasySockets.Mock;

public class MockEasySocket : EasySocket
{
	public override Task OnMessage(string message)
	{
		throw new NotImplementedException();
	}
}