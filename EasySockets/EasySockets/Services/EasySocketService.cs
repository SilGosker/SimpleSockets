using EasySockets.Enums;
using EasySockets.Events;

namespace EasySockets.Services;

internal sealed class EasySocketService : IEasySocketService
{
    private readonly List<EasySocketRoom> _rooms = new();

    public bool Any()
    {
        return _rooms.Count > 0;
    }

    public bool Any(string roomId)
    {
        return _rooms.Any(e => e.Id == roomId);
    }

    public bool Any(string roomId, string clientId)
    {
        return _rooms.Any(e => e.Id == roomId && e.Sockets.Any(o => o.ClientId == clientId && o.IsConnected()));
    }

    public int Count()
    {
        return _rooms.Sum(e => e.Sockets.Count);
    }

    public int Count(string roomId)
    {
        return _rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets.Count ?? 0;
    }

    public Task ForceLeaveAsync(string roomId)
    {
        return ForceLeaveAsync(roomId, CancellationToken.None);
    }

    public Task ForceLeaveAsync(string roomId, CancellationToken cancellationToken)
    {
        return Task.WhenAll(_rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets
            .Select(e => e.CloseAsync(cancellationToken)) ?? Enumerable.Empty<Task>());
    }

    public Task ForceLeaveAsync(string roomId, string clientId)
    {
        return ForceLeaveAsync(roomId, clientId, CancellationToken.None);
    }

    public Task ForceLeaveAsync(string roomId, string clientId, CancellationToken cancellationToken)
    {
        return _rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets
                   .FirstOrDefault(e => e.IsConnected() && e.ClientId == clientId)?.CloseAsync(cancellationToken) ??
               Task.CompletedTask;
    }

    public Task SendToRoomAsync(string roomId, string message)
    {
        return SendToRoomAsync(roomId, message, CancellationToken.None);
    }

    public Task SendToRoomAsync(string roomId, string message, CancellationToken cancellationToken)
    {
        return Task.WhenAll(_rooms.Where(e => e.Id == roomId)
            .SelectMany(room => room.Sockets).Select(e => e.SendToClientAsync(message, cancellationToken)));
    }

    public Task SendToRoomAsync(string roomId, string @event, string message)
    {
        return SendToRoomAsync(roomId, @event, message, CancellationToken.None);
    }

    public Task SendToRoomAsync(string roomId, string @event, string message, CancellationToken cancellationToken)
    {
        return Task.WhenAll(_rooms.SingleOrDefault(e => e.Id == roomId)?.Sockets
                                .Where(e => e.IsConnected())
                                .Select(e => e is IEventSocket o
                                    ? o.SendToClientAsync(@event, message, cancellationToken)
                                    : Task.CompletedTask)
                            ?? Enumerable.Empty<Task>());
    }

    public Task SendToClientAsync(string roomId, string clientId, string message)
    {
        return SendToClientAsync(roomId, clientId, message, CancellationToken.None);
    }

    public Task SendToClientAsync(string roomId, string clientId, string message, CancellationToken cancellationToken)
    {
        return _rooms.FirstOrDefault(e => e.Id == roomId)
            ?.Sockets.FirstOrDefault(e => e.ClientId == clientId)
            ?.SendToClientAsync(message, cancellationToken) ?? Task.CompletedTask;
    }

    public Task SendToClientAsync(string roomId, string clientId, string @event, string message)
    {
        return SendToClientAsync(roomId, clientId, @event, message, CancellationToken.None);
    }

    public Task SendToClientAsync(string roomId, string clientId, string @event, string message,
        CancellationToken cancellationToken)
    {
        return Task.WhenAll(_rooms.FirstOrDefault(e => e.Id == roomId)?.Sockets
                                .Where(e => e.IsConnected() && e.ClientId == clientId)
                                .Select(e => e is IEventSocket o
                                    ? o.SendToClientAsync(@event, message, cancellationToken)
                                    : Task.CompletedTask)
                            ?? Enumerable.Empty<Task>());
    }

    public IEnumerable<IGrouping<string, IEasySocket>> GetGroupings()
    {
        return _rooms.SelectMany(e => e.Sockets).GroupBy(e => e.RoomId);
    }

    internal async Task AddSocket(IEasySocket socket)
    {
        if (!socket.IsConnected()) return;

        socket.Emit = BroadCast;
        socket.DisposeAtSocketHandler = RemoveSocket;

        var room = _rooms.FirstOrDefault(e => e.Id == socket.RoomId);
        if (room == null)
            _rooms.Add(new EasySocketRoom(socket.RoomId, socket));
        else
            room.Sockets.Add(socket);

        await socket.OnConnect().ConfigureAwait(false);
        await socket.ReceiveMessagesAsync().ConfigureAwait(false);
    }

    private Task BroadCast(IEasySocket? sender, BroadCastFilter broadCastFilter, string? message)
    {
        if (string.IsNullOrEmpty(message) || sender == null || broadCastFilter == BroadCastFilter.Everyone)
            return Task.CompletedTask;

        var simpleSockets = _rooms.SelectMany(e => e.Sockets);
        if (broadCastFilter.HasFlag(BroadCastFilter.EqualRoomId))
            simpleSockets = _rooms.FirstOrDefault(room => room.Id == sender.RoomId)?.Sockets ??
                            Enumerable.Empty<IEasySocket>();

        if (broadCastFilter.HasFlag(BroadCastFilter.EqualType))
            simpleSockets = simpleSockets.Where(e => e.GetType() == sender.GetType());

        if (broadCastFilter.HasFlag(BroadCastFilter.Members))
            simpleSockets = simpleSockets.Where(e => e.ClientId != sender.ClientId);

        return Task.WhenAll(simpleSockets.Select(e => e.SendToClientAsync(message)));
    }

    internal void RemoveSocket(IEasySocket caster)
    {
        var room = _rooms.FirstOrDefault(e => e.Id == caster.RoomId);

        if (room == null) return;

        room.Sockets.RemoveAll(e => e.ClientId == caster.ClientId);
        if (room.Sockets.Count < 1) _rooms.Remove(room);
    }
}