namespace SimpleSockets.DataModels
{

    internal class SimpleSocketRoom
    {
        internal SimpleSocketRoom(string id, SimpleSocket socket)
        {
            Id = id;
            Sockets = new List<SimpleSocket> { socket };
        }
        internal string Id { get; set; }
        internal List<SimpleSocket> Sockets { get; set; }

    }
}