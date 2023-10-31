using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using EasySockets.Builder;
using EasySockets.Enums;

namespace EasySockets;

[DebuggerDisplay("{RoomId}.{UserId} = {_webSocket.State}")]
public abstract class EasySocket : IEasySocket
{
    private readonly CancellationTokenSource _cts;
    private readonly EasySocketOptions _options;
    private readonly WebSocket _webSocket;
    private bool _isDisposed;
    private string _roomId = null!;
    private string _userId = null!;
    private bool _isReceiving;
    protected internal EasySocket(WebSocket webSocket, EasySocketOptions options)
    {
        _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        _cts = new CancellationTokenSource();
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Func<IEasySocket, BroadCastFilter, string, Task>? Emit { get; set; } = null!;
    public Action<IEasySocket>? DisposeAtSocketHandler { get; set; } = null!;

    public string RoomId
    {
        get => _roomId;
        set
        {
            if (_roomId != null)
                throw new InvalidOperationException("RoomId cannot be set after the middleware initialized it");
            _roomId = value;
        }
    }

    public string UserId
    {
        get => _userId;
        set
        {
            if (_userId != null)
                throw new InvalidOperationException("UserId cannot be set after the middleware initialized it");
            _userId = value;
        }
    }

    public bool IsConnected()
    {
        return _webSocket.State == WebSocketState.Open;
    }

    /// <summary>
    ///     Sends a message to the client websocket.
    /// </summary>
    /// <param name="message">The message to be sent.</param>
    /// <returns>A task representing the asynchronous operation of sending the message to the client.</returns>
    public async Task SendToClient(string message)
    {
        if (!IsConnected())
        {
            await DisposeAsync();
            return;
        }

        var bytes = _options.Encoding.GetBytes(message);
        try
        {
            await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
        }
        catch (WebSocketException)
        {
            await DisposeAsync();
        }
    }

    public async Task ReceiveMessages()
    {
        if (_isReceiving) return;
        _isReceiving = true;

        StringBuilder sb = new();
        var chunks = new byte[_options.ChunkSize];
        while (IsConnected() && !_cts.IsCancellationRequested && !_isDisposed)
        {
            WebSocketReceiveResult result = new(0, WebSocketMessageType.Text, false);

            while (!result.EndOfMessage)
                try
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(chunks), _cts.Token);
                    sb.Append(_options.Encoding.GetString(chunks.AsSpan()).AsSpan().TrimEnd('\0'));
                }
                catch (WebSocketException)
                {
                    break;
                }


            if (result.EndOfMessage && IsConnected()) await OnMessage(sb.ToString());

            sb = sb.Clear();

            for (var i = 0; i < chunks.Length; i++) chunks[i] = default;
        }

        await DisposeAsync();
    }

    public virtual Task OnConnect()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnDisconnect()
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed) return;
        GC.SuppressFinalize(this);

        try
        {
            _cts.Cancel();
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, _options.ClosingStatusDescription,
                CancellationToken.None);
            _webSocket.Abort();
        }
        finally
        {
            _cts.Dispose();
            _webSocket.Dispose();
            _isDisposed = true;
            try
            {
                await OnDisconnect();
            }
            finally
            {
                DisposeAtSocketHandler?.Invoke(this);
            }
        }
    }

    /// <summary>
    ///     Sends a message to all members of the sockets room matching the <see cref="RoomId" />
    /// </summary>
    /// <inheritdoc cref="Broadcast(EasySockets.Enums.BroadCastFilter,string)" />
    public Task Broadcast(string message)
    {
        return Emit?.Invoke(this, BroadCastFilter.RoomMembers, message) ?? Task.CompletedTask;
    }

    /// <summary>
    ///     Sends a message to the members matching the all members specified by the <paramref name="filter" />
    /// </summary>
    /// <param name="filter">The broadcast level the message will reach</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task Broadcast(BroadCastFilter filter, string message)
    {
        return Emit?.Invoke(this, filter, message) ?? Task.CompletedTask;
    }

    public abstract Task OnMessage(string message);
}