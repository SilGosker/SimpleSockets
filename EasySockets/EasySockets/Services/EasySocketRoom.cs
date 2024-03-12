namespace EasySockets.Services;

internal sealed class EasySocketRoom
{
    internal EasySocketRoom(string id, IEasySocket socket)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Sockets = new List<IEasySocket> { socket ?? throw new ArgumentNullException(nameof(socket)) };
    }

    internal string Id { get; set; }
    internal List<IEasySocket> Sockets { get; set; }
}