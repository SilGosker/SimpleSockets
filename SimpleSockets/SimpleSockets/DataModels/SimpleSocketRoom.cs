namespace SimpleSockets.DataModels
{

    internal class SimpleSocketRoom
    {
        internal string Id { get; set; } = null!;
        internal List<SimpleSocket> Sockets { get; set; } = new List<SimpleSocket>();

    }
}