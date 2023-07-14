using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.Services;

namespace SimpleSockets.Middleware
{
    internal class SocketMiddleware
    {
        private static readonly Dictionary<string, Type> TypeDictionary = new Dictionary<string, Type>();
        private readonly RequestDelegate _next;
        private readonly SimpleSocketService _simpleSocketService;
        private readonly IServiceScopeFactory _scopeFactory;

        internal static void SetBehavior<T>(string url) where T : SimpleSocket
        {
            TypeDictionary[url] = typeof(T);
        }

        public SocketMiddleware(RequestDelegate next, SimpleSocketService simpleSocketService,
            IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _simpleSocketService = simpleSocketService;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }


            Type? type = TypeDictionary.GetValueOrDefault(context.Request.Path.ToString());
            if (type == null)
            {
                await _next.Invoke(context);
                return;
            }


            var methodInfo = type.GetMethod("OnAuthentication", BindingFlags.Static);
            if (methodInfo == null)
            {
                await _next.Invoke(context);
                return;
            }

            if (!await (Task<bool>)(methodInfo.Invoke(null, new object?[] {context}) ?? Task.FromResult(true)))
            {
                return;
            }

            string genericProtocol = context.WebSockets.WebSocketRequestedProtocols[0];
            SimpleSocket simpleSocket;
            using (var scope = _scopeFactory.CreateScope())
            {
                simpleSocket = scope.ServiceProvider.GetRequiredService<SimpleSocket>(await context.WebSockets.AcceptWebSocketAsync(genericProtocol));
            }

            if (simpleSocket == null)
            {
                await _next.Invoke(context);
                return;
            }

            if (string.IsNullOrEmpty(simpleSocket.UserId))
            {
                simpleSocket.UserId = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(simpleSocket.RoomId))
            {
                simpleSocket.RoomId = genericProtocol;
            }

            await _simpleSocketService.AddSocket(simpleSocket);
        }
    }
}