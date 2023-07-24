using SimpleSockets.Interfaces;

namespace SimpleSockets.DataModels
{

    internal sealed class SimpleSocketRoom
    {
        internal SimpleSocketRoom(string id, ISimpleSocket socket)
        {
            Id = id;
            Sockets = new List<ISimpleSocket> { socket };
        }
        internal string Id { get; set; }
        internal List<ISimpleSocket> Sockets { get; set; }

    }
}