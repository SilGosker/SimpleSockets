using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using EasySockets.Builder;
using EasySockets.Enums;

namespace EasySockets;

[DebuggerDisplay("{ClientId} = {_webSocket.State}")]
public abstract class EasySocket : IEasySocket
{
    private readonly CancellationTokenSource _cts = new();
    private EasySocketOptions _options = null!;
    private WebSocket _webSocket = null!;
    private bool _isDisposed;
    private bool _isReceiving;

    string IInternalEasySocket.RoomId
    {
        set => RoomId = value;
    }

    string IInternalEasySocket.ClientId
    {
        set => ClientId = value;
    }

    WebSocket IInternalEasySocket.WebSocket
    {
        set => _webSocket = value;
    }

    EasySocketOptions IInternalEasySocket.Options
    {
        set => _options = value;
    }

    private Func<IEasySocket, BroadCastFilter, string, Task>? _emit;

    private Action<IEasySocket>? _disposeAtSocketHandler;

    public string RoomId { get; private set; } = null!;

    public string ClientId { get; private set; } = null!;

    Func<IEasySocket, BroadCastFilter, string, Task>? IInternalEasySocket.Emit
    {
        set => _emit = value;
    }

    Action<IEasySocket>? IInternalEasySocket.DisposeAtSocketHandler
    {
        set => _disposeAtSocketHandler = value;
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
    public async Task SendToClientAsync(string message)
    {
        if (!IsConnected())
        {
            await CloseAsync().ConfigureAwait(false);
            return;
        }

        Encoder encoder = _options.Encoding.GetEncoder();
        var byteBuffer = new byte[_options.ChunkSize];
        int charsProcessed = 0;

        try
        {
            while (charsProcessed < message.Length)
            {
                bool flush = charsProcessed + _options.ChunkSize >= message.Length;
                encoder.Convert(message.AsSpan()[charsProcessed..], byteBuffer, flush, out int charsUsed, out int bytesUsed, out _);

                await _webSocket
                    .SendAsync(new ArraySegment<byte>(byteBuffer, 0, bytesUsed), WebSocketMessageType.Text, flush, _cts.Token)
                    .ConfigureAwait(false);

                charsProcessed += charsUsed;
            }
        }
        catch (WebSocketException)
        {
            await CloseAsync().ConfigureAwait(false);
        }
        
    }


    public async Task CloseAsync()
    {
        await OnDisconnect().ConfigureAwait(false);
        try
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, _options.ClosingStatusDescription,
                CancellationToken.None).ConfigureAwait(false);
            _webSocket.Abort();
        }
        catch (WebSocketException)
        {
            // ignored
        }

        Dispose();
    }

    public async Task ReceiveMessagesAsync()
    {
        if (_isReceiving) return;
        _isReceiving = true;

        StringBuilder sb = new();
        var buffer = new byte[_options.ChunkSize];

        while (IsConnected() && !_cts.IsCancellationRequested && !_isDisposed)
        {
            WebSocketReceiveResult result = new(0, WebSocketMessageType.Text, false);

            while (!result.EndOfMessage)
                try
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token).ConfigureAwait(false);

                    if (result.MessageType != WebSocketMessageType.Text) continue;
                    
                    sb.Append(_options.Encoding.GetString(buffer.AsSpan()[..result.Count]));
                }
                catch (WebSocketException)
                {
                    break;
                }

            if (result.EndOfMessage && IsConnected())
            {
                await OnMessage(sb.ToString()).ConfigureAwait(false);
                sb.Clear();
            }
        }

        await CloseAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     The method invoked after the client successfully connected to the server.
    /// </summary>
    /// <returns>The task representing the asynchronous operation</returns>
    public virtual Task OnConnect()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     The method invoked after the client is disconnected from the server.
    /// </summary>
    /// <returns>The task representing the asynchronous operation</returns>
    public virtual Task OnDisconnect()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Sends a message to all members of the sockets room matching the <see cref="RoomId" />
    /// </summary>
    /// <inheritdoc cref="Broadcast(BroadCastFilter, string)" />
    public Task Broadcast(string message)
    {
        return _emit?.Invoke(this, BroadCastFilter.RoomMembers, message) ?? Task.CompletedTask;
    }

    /// <summary>
    ///     Sends a message to the members matching the all members specified by the <paramref name="filter" />
    /// </summary>
    /// <param name="filter">The broadcast level the message will reach</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task Broadcast(BroadCastFilter filter, string message)
    {
        return _emit?.Invoke(this, filter, message) ?? Task.CompletedTask;
    }

    /// <summary>
    ///     The method invoked when a message is received from the client.
    /// </summary>
    /// <param name="message">The message as a string received from the client.</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public abstract Task OnMessage(string message);

    public void Dispose()
    {
        if (_isDisposed) return;
        try
        {
            GC.SuppressFinalize(this);
            _cts.Cancel();
            _cts.Dispose();
            _webSocket.Dispose();
            _disposeAtSocketHandler?.Invoke(this);
            _isDisposed = true;
        }
        catch
        {
            // ignored
        }
    }
}