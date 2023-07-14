using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using SimpleSockets.DataModels;
using  Microsoft.AspNetCore.Http;

namespace SimpleSockets;

public delegate Task<bool> AuthenticationDelegate(HttpContext context);

[DebuggerDisplay("{RoomId}.{UserId} is: {_webSocket.State}")]
    public abstract class SimpleSocket : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private bool _isDisposed;
        public static AuthenticationDelegate Authenticate;
        protected SimpleSocket()
        {
            _cts = new CancellationTokenSource();
        }

        public Func<SimpleSocket, SimpleSocketMessage, Task> Emit { get; set; }
        public Action<SimpleSocket> DisposeAtSocketHandler { get; set; }
        public string RoomId { get; set; }
        public string UserId { get; set; }
        private WebSocket _webSocket;

        public bool IsConnected()
        {
            return _webSocket.State == WebSocketState.Open;
        }

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
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                    _cts.Token);
            }
            catch (WebSocketException)
            {
                await Leave();
            }
        }

        public async Task ReceiveMessage()
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
                bytes = new byte[1024 * 4];
            }

            await Leave();
        }

        public void SetRecourse(object resource)
        {
            this._webSocket = (WebSocket)resource;
        }

        public async Task Leave()
        {
            try
            {
                _webSocket.Abort();
            }
            finally
            {
                await OnClose();
                Dispose();
            }
        }

        public static Task<bool> OnAuthentication(HttpContext context)
        {
            return Authenticate(context);
        }
        public abstract Task OnConnect();

        public abstract Task OnClose();

        public abstract Task OnMessage(string message);

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
                DisposeAtSocketHandler(this);
            }

            _isDisposed = true;
        }
    }