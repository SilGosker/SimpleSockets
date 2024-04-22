using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EasySockets.Authentication;
using EasySockets.Builder;
using EasySockets.Events;
using Microsoft.Extensions.Options;
using EasySockets.Services.Caching;

namespace EasySockets.Services;

internal sealed class EasySocketAuthenticationService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly EasySocketGlobalOptions _options;
    public EasySocketAuthenticationService(IServiceScopeFactory serviceScopeFactory, IOptions<EasySocketGlobalOptions> options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
    }

    internal async Task<EasySocketAuthenticationResult> GetAuthenticationResultAsync(EasySocketTypeCache easySocketTypeCache, HttpContext context)
    {
        var roomId = _options.GetDefaultRoomId(context);
        var clientId = _options.GetDefaultClientId(context);

        var scope = context.RequestServices;

        EasySocketAuthenticationResult authenticationResult = new(
            easySocketTypeCache.Options.Authenticators.Count == 0,
            roomId,
            clientId);

        foreach (var authenticatorType in easySocketTypeCache.Options.Authenticators)
        {
            var authenticator = ActivatorUtilities.CreateInstance(scope, authenticatorType);

            authenticationResult = authenticator switch
            {
                IEasySocketAsyncAuthenticator asyncAuthenticator => await asyncAuthenticator.AuthenticateAsync(authenticationResult, context),
                IEasySocketAuthenticator syncAuthenticator => syncAuthenticator.Authenticate(authenticationResult, context),
                _ => authenticationResult
            };

            if (authenticationResult.IsAuthenticated != true) return authenticationResult;
        }

        authenticationResult.RoomId ??= roomId ?? throw new InvalidOperationException("The authenticationResult.RoomId and the default roomId should not be null after successful authentication");
        authenticationResult.ClientId ??= clientId ?? throw new InvalidOperationException("The authenticationResult.ClientId and the default userId should not be null after successful authentication");

        return authenticationResult;
    }

    internal async Task<IEasySocket?> GetInstanceAsync(EasySocketTypeCache cache, WebSocketManager websockets, string roomId, string clientId)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var ws = websockets.WebSocketRequestedProtocols.Count > 0
            ? await websockets.AcceptWebSocketAsync(websockets.WebSocketRequestedProtocols[0])
            : await websockets.AcceptWebSocketAsync();

        if (ws == null) return null;

        if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, cache.EasySocketType) is not IEasySocket easySocket)
            return null;

        easySocket.WebSocket = ws;
        ((IInternalEasySocket)easySocket).RoomId = roomId;
        ((IInternalEasySocket)easySocket).ClientId = clientId;
        easySocket.Options = cache.Options;

        if (easySocket is IInternalEventSocket eventSocket)
        {
            eventSocket.Events = ((EventSocketTypeCache)cache).EventInfos;
        }

        return easySocket;
    }
}