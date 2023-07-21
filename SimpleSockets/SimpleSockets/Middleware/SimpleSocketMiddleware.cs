using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.DataModels;
using SimpleSockets.Interfaces;
using SimpleSockets.Services;

namespace SimpleSockets.Middleware
{
    internal class SocketMiddleware
    {
        private static readonly Dictionary<string, SimpleSocketTypeCaching> TypeDictionary = new();
        private readonly RequestDelegate _next;
        private readonly SimpleSocketService _simpleSocketService;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly SimpleSocketAuthenticationResult DefaultAuthenticationResult =
            new SimpleSocketAuthenticationResult(true, null, null);
        internal static void SetBehavior<TSimpleSocket>(string url)
            where TSimpleSocket : ISimpleSocket
        {
            TypeDictionary[url] = SimpleSocketTypeCaching.Create<TSimpleSocket>();
        }

        internal static void SetBehavior<TSimpleSocket, TSimpleSocketAuthenticator>(string url)
            where TSimpleSocket : ISimpleSocket
            where TSimpleSocketAuthenticator : ISimpleSocketAuthenticator
        {
            TypeDictionary[url] = SimpleSocketTypeCaching.Create<TSimpleSocket, TSimpleSocketAuthenticator>();
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

            var simpleSocketType = TypeDictionary.GetValueOrDefault(context.Request.Path.ToString());
            if (simpleSocketType == null)
            {
                await _next.Invoke(context);
                return;
            }
            
            SimpleSocketAuthenticationResult authenticationResult = DefaultAuthenticationResult;
            if (simpleSocketType.AuthenticatorType != null)
            {
                using var scope = _scopeFactory.CreateScope();
                
                if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, simpleSocketType.AuthenticatorType) is ISimpleSocketAuthenticator authenticator)
                {
                    authenticationResult = await authenticator.Authenticate();
                }
            }

            if (authenticationResult.IsAuthenticated != true)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var genericProtocol = context.WebSockets.WebSocketRequestedProtocols[0];
            ISimpleSocket simpleSocket;
            using (var scope = _scopeFactory.CreateScope())
            {
                simpleSocket = (ISimpleSocket)ActivatorUtilities.CreateInstance(scope.ServiceProvider,
                    simpleSocketType.SimpleSocketType, await context.WebSockets.AcceptWebSocketAsync(genericProtocol));
            }

            simpleSocket.UserId = authenticationResult.UserId ?? Guid.NewGuid().ToString();
            simpleSocket.RoomId = authenticationResult.RoomId ?? "__0";

            await _simpleSocketService.AddSocket(simpleSocket);
        }
    }
}