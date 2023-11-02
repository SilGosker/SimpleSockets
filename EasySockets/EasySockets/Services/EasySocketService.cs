using EasySockets.Enums;
using EasySockets.Events;

namespace EasySockets.Services;

internal sealed class EasySocketService : IEasySocketService
{
    private readonly List<EasySocketRoom> _rooms;
    private static EasySocketService? _instance;

    internal static EasySocketService Create()
    {
        return _instance ??= new EasySocketService();
    }

    private EasySocketService()
    {
        _rooms = new List<EasySocketRoom>();
    }

    internal async Task AddSocket(IEasySocket socket)
    {
        if (!socket.IsConnected()) return;

        socket.Emit = BroadCast;
        socket.DisposeAtSocketHandler = RemoveSocket;

        var room = _rooms.SingleOrDefault(e => e.Id == socket.RoomId);
        if (room == null)
        {
            _rooms.Add(new EasySocketRoom(socket.RoomId, socket));
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
        return _rooms.Any(e => e.Id == roomId && e.Sockets.Any(o => o.ClientId == userId && o.IsConnected()));
    }

    public bool Any(string roomId)
    {
        return _rooms.Any(e => e.Id == roomId);
    }

    public int Count(string roomId)
    {
        return _rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets.Count ?? 0;
    }

    public int Count()
    {
        return _rooms.Sum(e => e.Sockets.Count);
    }

    public Task ForceLeave(string roomId)
    {
        return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
            .Select(e => e.DisposeAsync().AsTask()) ?? Enumerable.Empty<Task>());
    }

    public Task ForceLeave(string roomId, string userId)
    {
        return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                .Where(e => e.IsConnected() && e.ClientId == userId)
                                .Select(e => e.DisposeAsync().AsTask())
                            ?? Enumerable.Empty<Task>());
    }

    public Task SendToRoom(string roomId, string @event, string message)
    {
        return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                .Where(e => e.IsConnected())
                                .Select(e => e is IEventSocket o ? o.SendToClient(message, @event) : Task.CompletedTask)
                            ?? Enumerable.Empty<Task>());
    }

    public async Task SendToRoom(string roomId, string message)
    {
        var tasks =
            (_rooms.Where(e => e.Id == roomId)
                .SelectMany(room => room.Sockets, (_, behavior) => behavior.SendToClient(message)));
        await Task.WhenAll(tasks);
    }

    public Task SendToClient(string roomId, string userId, string @event, string message)
    {
        return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                .Where(e => e.IsConnected() && e.ClientId == userId)
                                .Select(e => e is IEventSocket o ? o.SendToClient(message, @event) : Task.CompletedTask)
                            ?? Enumerable.Empty<Task>());
    }
    public async Task SendToClient(string roomId, string userId, string message)
    {
        await _rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets.FirstOrDefault(e => e.ClientId == userId)
            ?.SendToClient(message)!;
    }

    public IEnumerable<IGrouping<string, IEasySocket>> GetGroupings()
    {
	    return _rooms.SelectMany(e => e.Sockets).GroupBy(e => e.RoomId);
    }

    private Task BroadCast(IEasySocket? sender, BroadCastFilter broadCastFilter, string? message)
    {
        if (string.IsNullOrEmpty(message) || sender == null || broadCastFilter == BroadCastFilter.Everyone)
            return Task.CompletedTask;

        IEnumerable<IEasySocket> simpleSockets = _rooms.SelectMany(e => e.Sockets);
        if (broadCastFilter.HasFlag(BroadCastFilter.EqualRoomId))
        {
            simpleSockets = _rooms.SingleOrDefault(room => room.Id == sender.RoomId)?.Sockets ??
                            Enumerable.Empty<IEasySocket>();
        }

        if (broadCastFilter.HasFlag(BroadCastFilter.EqualType))
        {
            simpleSockets = simpleSockets.Where(e => e.GetType() == sender.GetType());
        }

        if (broadCastFilter.HasFlag(BroadCastFilter.Members))
        {
            simpleSockets = simpleSockets.Where(e => e.ClientId != sender.ClientId);
        }

        return Task.WhenAll(simpleSockets.Select(e => e.SendToClient(message)));
    }

    internal void RemoveSocket(IEasySocket? caster)
    {
        if (caster == null) return;

        var room = _rooms.SingleOrDefault(e => e.Id == caster.RoomId);
        if (room == null) return;

        room.Sockets.RemoveAll(e => e.ClientId == caster.ClientId && !e.IsConnected());
        if (room.Sockets.All(e => !e.IsConnected()))
        {
            _rooms.Remove(room);
        }
    }
}