using Microsoft.AspNetCore.Http;
using EasySockets.Services;
using EasySockets.Services.Caching;

namespace EasySockets.Middleware;

internal sealed class SocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly EasySocketService _easySocketService;
    private readonly EasySocketAuthenticationService _easySocketAuthenticator;
    private readonly EasySocketTypeHolder _easySocketTypeHolder;

    public SocketMiddleware(RequestDelegate next, EasySocketService easySocketService, EasySocketAuthenticationService easySocketAuthenticator, EasySocketTypeHolder easySocketTypeHolder)
    {
        _next = next;
        _easySocketService = easySocketService;
        _easySocketAuthenticator = easySocketAuthenticator;
        _easySocketTypeHolder = easySocketTypeHolder;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next.Invoke(context);
            return;
        }

        if (!_easySocketTypeHolder.TryGetValue(context.Request.Path, out var easySocketTypeCache))
        {
            await _next.Invoke(context);
            return;
        }

        var authenticationResult = await _easySocketAuthenticator.GetAuthenticationResultAsync(
            easySocketTypeCache,
            context);

        if (!authenticationResult.IsAuthenticated)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var easySocket = await _easySocketAuthenticator.GetInstanceAsync(easySocketTypeCache, context.WebSockets, authenticationResult.RoomId!, authenticationResult.ClientId!);

        if (easySocket == null)
        {
            await _next.Invoke(context);
            return;
        }

        await _easySocketService.AddSocket(easySocket);
        _easySocketService.RemoveSocket(easySocket);
    }
}