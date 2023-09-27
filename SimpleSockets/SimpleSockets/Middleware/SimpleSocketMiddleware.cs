using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.Authentication;
using SimpleSockets.Builder;
using SimpleSockets.DataModels;
using SimpleSockets.Services;

namespace SimpleSockets.Middleware;

internal sealed class SocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SimpleSocketService _simpleSocketService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SimpleSocketMiddlewareOptions _options;

    public SocketMiddleware(RequestDelegate next, SimpleSocketService simpleSocketService, IServiceScopeFactory scopeFactory, SimpleSocketMiddlewareOptions options)
    {
        _next = next;
        _simpleSocketService = simpleSocketService;
        _scopeFactory = scopeFactory;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next.Invoke(context);
            return;
        }

        var simpleSocket = await SimpleSocketInstanceFactory.GetAuthenticatedInstance(context,
            _options.IsDefaultAuthenticated,
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract : always expect the worst from your users
            _options.GetDefaultRoomId(context) ?? "__0",
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract : always expect the worst from your users
            _options.GetDefaultUserId(context) ?? Guid.NewGuid().ToString());

        if (simpleSocket == null)
        {
            context.Response.StatusCode = 401;
            return;
        }

        await _simpleSocketService.AddSocket(simpleSocket);
        _simpleSocketService.RemoveSocket(simpleSocket);
    }
}