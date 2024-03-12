using EasySockets;

namespace TestApplication.Websockets;

public class ChatSocket : EasySocket
{
    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }
}