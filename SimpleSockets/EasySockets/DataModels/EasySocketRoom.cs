using EasySockets.Interfaces;

namespace EasySockets.DataModels;

internal sealed class EasySocketRoom
{
    internal EasySocketRoom(string id, IEasySocket socket)
    {
        Id = id;
        Sockets = new List<IEasySocket> { socket };
    }

    internal string Id { get; set; }
    internal List<IEasySocket> Sockets { get; set; }
}