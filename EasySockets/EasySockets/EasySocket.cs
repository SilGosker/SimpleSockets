using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using EasySockets.Builder;
using EasySockets.Enums;

namespace EasySockets;

[DebuggerDisplay("{ClientId} = {_webSocket.State}")]
public abstract class EasySocket : IEasySocket
{
    private EasySocketOptions _options = null!;
    private WebSocket _webSocket = null!;
    private readonly Queue<string> _messagePipeline = new();
    private int _bufferCharCount;

    private Action<IEasySocket>? _disposeAtSocketHandler;

    private Func<IEasySocket, BroadCastFilter, string, Task>? _emit;
    private bool _isDisposed;
    private bool _isReceiving;
    private bool _isSending;
    private byte[] _sendBuffer = Array.Empty<byte>();

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
        set
        {
            _options = value;
            _sendBuffer = new byte[_options.SendBufferSize];
            _bufferCharCount = _options.Encoding.GetMaxCharCount(_options.SendBufferSize);
        }
    }

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
    public Task SendToClientAsync(string message)
    {
        return SendToClientAsync(message, CancellationToken.None);
    }

    public Task SendToClientAsync(string message, CancellationToken cancellationToken)
    {
        if (!IsConnected()) return Task.CompletedTask;

        _messagePipeline.Enqueue(message);
        return StartSendingMessageAsync(cancellationToken);
    }

    public Task CloseAsync()
    {
        return CloseAsync(CancellationToken.None);
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        await OnDisconnect().ConfigureAwait(false);
        try
        {
            await _webSocket
                .CloseAsync(WebSocketCloseStatus.NormalClosure, _options.ClosingStatusDescription, cancellationToken)
                .ConfigureAwait(false);
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
        var buffer = new byte[_options.ReceiveBufferSize];

        while (IsConnected() && !_isDisposed)
        {
            WebSocketReceiveResult result = new(0, WebSocketMessageType.Text, false);

            while (!result.EndOfMessage)
                try
                {
                    result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None)
                        .ConfigureAwait(false);

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

    public void Dispose()
    {
        if (_isDisposed) return;
        try
        {
            GC.SuppressFinalize(this);
            _webSocket.Dispose();
            _disposeAtSocketHandler?.Invoke(this);
            _isDisposed = true;
        }
        catch
        {
            // ignored
        }
    }

    private async Task StartSendingMessageAsync(CancellationToken cancellationToken)
    {
        if (_isSending || !IsConnected()) return;

        _isSending = true;
        var encoder = _options.Encoding.GetEncoder();

        while (_messagePipeline.Count > 0)
        {
            var message = _messagePipeline.Dequeue();

            var charsProcessed = 0;

            try
            {
                while (charsProcessed < message.Length)
                {
                    var flush = charsProcessed + _bufferCharCount >= message.Length;
                    encoder.Convert(message.AsSpan()[charsProcessed..], _sendBuffer, flush, out var charsUsed,
                        out var bytesUsed, out _);

                    await _webSocket
                        .SendAsync(new ArraySegment<byte>(_sendBuffer, 0, bytesUsed), WebSocketMessageType.Text, flush,
                            cancellationToken)
                        .ConfigureAwait(false);

                    charsProcessed += charsUsed;
                }

                encoder.Reset();
            }
            catch (WebSocketException)
            {
                await CloseAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        _isSending = false;
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
}