namespace EasySockets;

internal sealed class EasySocketMessage
{
    public EasySocketMessage(string message)
    {
        Message = message;
    }
    internal string Message { get; set; }
    internal CancellationToken CancellationToken { get; set; }
}