using SimpleSockets.DataModels;
using SimpleSockets.Enums;

namespace SimpleSockets.Services
{
    public class SimpleSocketService
    {
        private readonly List<SimpleSocketRoom> _rooms;

        public SimpleSocketService()
        {
            _rooms = new List<SimpleSocketRoom>();
        }

        public async Task AddSocket(SimpleSocket socket)
        {
            if (!socket.IsConnected()) return;
            SimpleSocketRoom? room = _rooms.SingleOrDefault(e => e.Id == socket.RoomId);

            socket.Emit = BroadCast;
            socket.DisposeAtSocketHandler = RemoveSocket;
            if (room == null)
            {
                SimpleSocketRoom newRoom = new SimpleSocketRoom()
                {
                    Id = socket.RoomId,
                    Sockets = new List<SimpleSocket> { socket }
                };
                _rooms.Add(newRoom);
            }
            else
            {
                room.Sockets.Add(socket);
            }

            await socket.OnConnect();
            await socket.ReceiveMessage();
        }

        public bool Any(string roomId, string userId)
        {
            return _rooms.Any(e => e.Id == roomId && e.Sockets.Any(o => o.UserId == userId && o.IsConnected()));
        }

        public Task ForceLeave(string roomId, string userId)
        {
            return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                    .Where(e => e.IsConnected() && e.UserId == userId)
                                    .Select(e => e.Leave()) 
                                ?? Enumerable.Empty<Task>());
        }

        public async Task RemoveSocket(string id)
        {
            List<Task> tasks =
                (from room in _rooms.Where(e => e.Sockets.Any(o => o.UserId == id))
                    from socket in room.Sockets.Where(socket => socket.UserId == id)
                    select Task.Run(() => room.Sockets.Remove(socket))).Cast<Task>().ToList();
            await Task.WhenAll(tasks);
        }

        public async Task SendToRoom(string roomId, string message)
        {
            List<Task> tasks =
                (from room in _rooms.Where(e => e.Id == roomId)
                    from behavior in room.Sockets
                    select behavior.SendMessage(message)).ToList();
            await Task.WhenAll(tasks);
        }

        public async Task SendToUser(string roomId, string userId, string message)
        {
            await _rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets.FirstOrDefault(e => e.UserId == userId)
                ?.SendMessage(message)!;
        }

        private async Task BroadCast(SimpleSocket? sender, SimpleSocketMessage? dataModelEmitWsMessage)
        {
            if (dataModelEmitWsMessage == null || sender == null)
                return;
            

            SimpleSocketRoom? room = _rooms.FirstOrDefault(e => e.Id == sender.RoomId);
            if (room == null)
                return;
            

           
            if (dataModelEmitWsMessage.BroadCastLevel > BroadCastLevel.EveryOne) return;
            string messageToSend = (!string.IsNullOrEmpty(dataModelEmitWsMessage.Event) ? ":" + dataModelEmitWsMessage.Event + ":" : "") +
                                   dataModelEmitWsMessage.Message;

            switch (dataModelEmitWsMessage.BroadCastLevel)
            {
                case 0 or BroadCastLevel.EveryOne:
                {
                    IEnumerable<Task> tasks = room.Sockets.Where(e => e.IsConnected()).Select(e => e.SendMessage(messageToSend));
                    await Task.WhenAll(tasks);
                    return;
                }
                case BroadCastLevel.Receiver:
                    await (room.Sockets.FirstOrDefault(e => e.IsConnected() && e.UserId == sender.UserId)?.SendMessage(messageToSend) ?? Task.CompletedTask);
                    return;
            }
        }

        private void RemoveSocket(SimpleSocket? caster)
        {
            if (caster == null) return;

            SimpleSocketRoom? room = _rooms.SingleOrDefault(e => e.Id == caster.RoomId);
            if (room == null) return;

            room.Sockets.RemoveAll(e => e.UserId == caster.UserId && !e.IsConnected());
            if (room.Sockets.All(e => !e.IsConnected()))
            {
                _rooms.Remove(room);
            }
        }
    }
}