using SimpleSockets.DataModels;
using SimpleSockets.Enums;

namespace SimpleSockets.Services
{
    internal class SimpleSocketService : ISimpleSocketService
    {
        private readonly List<SimpleSocketRoom> _rooms;

        public SimpleSocketService()
        {
            _rooms = new List<SimpleSocketRoom>();
        }

        internal async Task AddSocket(ISimpleSocket socket)
        {
            if (!socket.IsConnected()) return;

            socket.BroadCast = BroadCast;
            socket.DisposeAtSocketHandler = RemoveSocket;

            var room = _rooms.SingleOrDefault(e => e.Id == socket.RoomId);
            if (room == null)
            {
                _rooms.Add(new SimpleSocketRoom(socket.RoomId, socket));
            }
            else
            {
                room.Sockets.Add(socket);
            }

            await socket.OnConnect();
            await socket.ReceiveMessages();
        }

        public bool Any(string roomId, string userId)
        {
            return _rooms.Any(e => e.Id == roomId && e.Sockets.Any(o => o.UserId == userId && o.IsConnected()));
        }

        public bool Any(string roomId)
        {
            return _rooms.Any(e => e.Id == roomId);
        }

        public Task ForceLeave(string roomId)
        {
            return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                .Select(e => e.Leave()) ?? Enumerable.Empty<Task>());
        }

        public Task ForceLeave(string roomId, string userId)
        {
            return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                    .Where(e => e.IsConnected() && e.UserId == userId)
                                    .Select(e => e.Leave())
                                ?? Enumerable.Empty<Task>());
        }

        public void Remove(string roomId)
        {
            _rooms.RemoveAll(e => e.Id == roomId);
        }

        public void Remove(string roomId, string userId)
        {
            _rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets.RemoveAll(e => e.UserId == userId);
        }

        public Task SendToRoom(string roomId, string @event, string message)
        {
            return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                    .Where(e => e.IsConnected())
                                    .Select(e => e.SendMessage(@event, message))
                                ?? Enumerable.Empty<Task>());
        }

        public async Task SendToRoom(string roomId, string message)
        {
            List<Task> tasks =
                (_rooms.Where(e => e.Id == roomId)
                    .SelectMany(room => room.Sockets, (_, behavior) => behavior.SendMessage(message))).ToList();
            await Task.WhenAll(tasks);
        }

        public Task SendToUser(string roomId, string userId, string @event, string message)
        {
            return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                    .Where(e => e.IsConnected() && e.UserId == userId)
                                    .Select(e => e.SendMessage(@event, message))
                                ?? Enumerable.Empty<Task>());
        }

        public async Task SendToUser(string roomId, string userId, string message)
        {
            await _rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets.FirstOrDefault(e => e.UserId == userId)
                ?.SendMessage(message)!;
        }

        private async Task BroadCast(ISimpleSocket? sender, BroadCastLevel broadCastLevel, string? message)
        {
            if (message == null || sender == null || broadCastLevel == 0)
                return;

            SimpleSocketRoom? room = _rooms.FirstOrDefault(e => e.Id == sender.RoomId);
            if (room == null)
                return;

            if (broadCastLevel > BroadCastLevel.EveryOne) return;
            
            switch (broadCastLevel)
            {
                default:
                case BroadCastLevel.EveryOne:
                {
                    IEnumerable<Task> tasks = room.Sockets.Where(e => e.IsConnected())
                        .Select(e => e.SendMessage(message));
                    await Task.WhenAll(tasks);
                    return;
                }
                case BroadCastLevel.Members:
                {
                    IEnumerable<Task> tasks = room.Sockets.Where(e => e.IsConnected() && e.UserId != sender.UserId)
                        .Select(e => e.SendMessage(message));
                    await Task.WhenAll(tasks);
                    return;
                }
            }
        }

        private void RemoveSocket(ISimpleSocket? caster)
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