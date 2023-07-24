using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.DataModels;
using SimpleSockets.Interfaces;
using SimpleSockets.Services;

namespace SimpleSockets.Middleware;

internal sealed class SocketMiddleware
{
    private static readonly Dictionary<string, SimpleSocketTypeCaching> TypeDictionary = new();
    private readonly RequestDelegate _next;
    private readonly SimpleSocketService _simpleSocketService;
    private readonly IServiceScopeFactory _scopeFactory;

    private static readonly SimpleSocketAuthenticationResult DefaultAuthenticationResult = new(true, null, null);
    internal static void SetBehavior<TSimpleSocket>(string url)
        where TSimpleSocket : ISimpleSocket
    {
        TypeDictionary[url] = SimpleSocketTypeCaching.Create<TSimpleSocket>();
    }

    internal static void SetBehavior<TSimpleSocket, TSimpleSocketAuthenticator>(string url)
        where TSimpleSocket : ISimpleSocket
        where TSimpleSocketAuthenticator : ISimpleSocketAsyncAuthenticator
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
            var authenticator = ActivatorUtilities.CreateInstance(scope.ServiceProvider, simpleSocketType.AuthenticatorType);
                
            authenticationResult = authenticator switch
            {
                ISimpleSocketAsyncAuthenticator asyncAuthenticator => await asyncAuthenticator.AuthenticateAsync(),
                ISimpleSocketAuthenticator syncAuthenticator => syncAuthenticator.Authenticate(),
                _ => authenticationResult
            };
        }

        if (authenticationResult.IsAuthenticated != true)
        {
            context.Response.StatusCode = 401;
            return;
        }

        ISimpleSocket simpleSocket;
        using (var scope = _scopeFactory.CreateScope())
        {
            var ws = context.WebSockets.WebSocketRequestedProtocols.Count > 0 
                ? await context.WebSockets.AcceptWebSocketAsync(context.WebSockets.WebSocketRequestedProtocols[0]) 
                : await context.WebSockets.AcceptWebSocketAsync();
            simpleSocket = (ISimpleSocket)ActivatorUtilities.CreateInstance(scope.ServiceProvider, simpleSocketType.SimpleSocketType, ws);
        }

        try
        {
            simpleSocket.UserId = authenticationResult.UserId ?? Guid.NewGuid().ToString();
            simpleSocket.RoomId = authenticationResult.RoomId ?? "__0";
        }
        catch (Exception)
        {
            throw new InvalidOperationException("The UserId or RoomId should not be set in the constructor of any SimpleSocket");
        }
        
        await _simpleSocketService.AddSocket(simpleSocket);
    }
}