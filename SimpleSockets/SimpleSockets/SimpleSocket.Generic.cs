using SimpleSockets.DataModels;
using SimpleSockets.Enums;
using System.Net.WebSockets;
using System.Text;

namespace SimpleSockets;

public abstract class SimpleSocket<TEvent> : IDisposable, ISimpleSocket
{
    private readonly CancellationTokenSource _cts;
    private bool _isDisposed;
    private readonly WebSocket _webSocket;
    public Func<ISimpleSocket, BroadCastLevel, string, Task>? BroadCast { get; set; } = null!;
    public Action<ISimpleSocket>? DisposeAtSocketHandler { get; set; } = null!;

    public string RoomId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    protected internal SimpleSocket(WebSocket webSocket)
    {
        _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        _cts = new CancellationTokenSource();
    }

    public Task Emit(string message) => BroadCast?.Invoke(this, BroadCastLevel.Members, message) ?? Task.CompletedTask;

    public Task Emit(string @event, string message) => BroadCast?.Invoke(this, BroadCastLevel.Members, CreateEvent(@event, message)) ?? Task.CompletedTask;

    public Task Emit(BroadCastLevel level, string message) => BroadCast?.Invoke(this, level, message) ?? Task.CompletedTask;

    public Task Emit(BroadCastLevel level, string @event, string message) => BroadCast?.Invoke(this, level, CreateEvent(@event, message)) ?? Task.CompletedTask;

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