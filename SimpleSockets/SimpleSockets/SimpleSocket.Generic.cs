using SimpleSockets.DataModels;
using SimpleSockets.Enums;
using SimpleSockets.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace SimpleSockets;

public abstract class SimpleSocket<TEvent> : IDisposable, ISimpleSocket
{
    private readonly CancellationTokenSource _cts;
    private bool _isDisposed;
    private readonly WebSocket _webSocket;
    public Func<ISimpleSocket, BroadCastLevel, string, Task>? Emit { get; set; } = null!;
    public Action<ISimpleSocket>? DisposeAtSocketHandler { get; set; } = null!;
    private string _roomId = null!;
    public string RoomId
    {
        get => _roomId;
        set
        {
            if (_roomId != null) throw new InvalidOperationException("RoomId cannot be set after the middleware initialized it");
            _roomId = value;
        }
    }
    private string _userId = null!;
    public string UserId
    {
        get => _userId;
        set
        {
            if (_userId != null) throw new InvalidOperationException("UserId cannot be set after the middleware initialized it");
            _userId = value;
        }
    }

    protected internal SimpleSocket(WebSocket webSocket)
    {
        _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        _cts = new CancellationTokenSource();
    }
    /// <summary>
    /// Sends a message to all members of the sockets room matching the <see cref="RoomId"/> 
    /// </summary>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task BroadCast(string message) => Emit?.Invoke(this, BroadCastLevel.RoomMembers, message) ?? Task.CompletedTask;
    /// <summary>
    /// Sends a message with an event id/name to all members of the sockets room matching the <see cref="RoomId"/> 
    /// </summary>
    /// <param name="event">The name/id of the event</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task BroadCast(string @event, string message) => Emit?.Invoke(this, BroadCastLevel.RoomMembers, CreateEvent(@event, message)) ?? Task.CompletedTask;

    /// <summary>
    /// Sends a message to the members matching the all members specified by the <paramref name="level"/>
    /// </summary>
    /// <param name="level">The broadcast level the message will reach</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task BroadCast(BroadCastLevel level, string message) => Emit?.Invoke(this, level, message) ?? Task.CompletedTask;
    /// <summary>
    /// Sends a message with an event id/name to all members specified by the <paramref name="level"/>
    /// </summary>
    /// <param name="level">The broadcast level the message will reach</param>
    /// <param name="event">The name/id of the event</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task BroadCast(BroadCastLevel level, string @event, string message) => Emit?.Invoke(this, level, CreateEvent(@event, message)) ?? Task.CompletedTask;

    public abstract string CreateEvent(string @event, string message);
    public abstract TEvent RegisterEvent(string message);

    public bool IsConnected() => _webSocket.State == WebSocketState.Open;

    public async Task SendMessage(string message)
    {
        if (!IsConnected())
        {
            await Leave();
            return;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(message);
        try
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (WebSocketException)
        {
            await Leave();
        }
    }

    public async Task SendMessage(string @event, string message)
    {
        if (!IsConnected())
        {
            await Leave();
            return;
        }

        byte[] bytes = Encoding.UTF8.GetBytes(CreateEvent(@event, message));
        try
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (WebSocketException)
        {
            await Leave();
        }
    }

    public async Task ReceiveMessages()
    {
        byte[] bytes = new byte[1024 * 4];
        while (IsConnected() && !_cts.IsCancellationRequested && !_isDisposed)
        {
            try
            {
                await _webSocket.ReceiveAsync(new ArraySegment<byte>(bytes), _cts.Token);
            }
            catch (WebSocketException)
            {
                break;
            }

            string message = Encoding.Default.GetString(bytes).Replace("\0", "");
            await OnMessage(message);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = default;
            }
        }

        await Leave();
    }

    public async Task Leave()
    {
        _cts.Cancel();
        try
        {
            _webSocket.Abort();
        }
        finally
        {
            await OnDisconnect();
            Dispose();
        }
    }

    public virtual Task OnConnect()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnDisconnect()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _cts.Cancel();
        GC.SuppressFinalize(this);
        try
        {
            _cts.Dispose();
            _webSocket.Dispose();
        }
        finally
        {
            DisposeAtSocketHandler!(this);
        }

        _isDisposed = true;
    }

    public abstract Task OnMessage(string message);
}