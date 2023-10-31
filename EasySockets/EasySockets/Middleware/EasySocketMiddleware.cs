using System.Net.NetworkInformation;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EasySockets.Authentication;
using EasySockets.Builder;
using EasySockets.DataModels;
using EasySockets.Services;

namespace EasySockets.Middleware;

internal sealed class SocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly EasySocketService _simpleSocketService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EasySocketMiddlewareOptions _options;

    public SocketMiddleware(RequestDelegate next, EasySocketService simpleSocketService, IServiceScopeFactory scopeFactory, EasySocketMiddlewareOptions options)
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

        var simpleSocket = await EasySocketInstanceFactory.GetAuthenticatedInstance(context,
            _options.IsDefaultAuthenticated,
            _options.GetDefaultRoomId,
            _options.GetDefaultUserId);

        if (simpleSocket == null)
        {
            context.Response.StatusCode = 401;
            return;
        }

        await _simpleSocketService.AddSocket(simpleSocket);
        _simpleSocketService.RemoveSocket(simpleSocket);
    }
}