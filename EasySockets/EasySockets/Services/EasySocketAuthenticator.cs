using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EasySockets.DataModels;
using EasySockets.Authentication;
using EasySockets.Builder;
using Microsoft.Extensions.Options;

namespace EasySockets.Services;

public class EasySocketAuthenticator
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOptions<EasySocketMiddlewareOptions> _options;
    public EasySocketAuthenticator(IServiceScopeFactory serviceScopeFactory, IOptions<EasySocketMiddlewareOptions> options)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options;
    }

    public async Task<EasySocketAuthenticationResult> GetAuthenticationResultAsync(EasySocketTypeCache easySocketTypeCache, HttpContext context)
    {
        var roomId = _options.Value.GetDefaultRoomId(context);
        var clientId = _options.Value.GetDefaultClientId(context);

        var scope = context.RequestServices;

        EasySocketAuthenticationResult authenticationResult = new(
            easySocketTypeCache.Options.IsDefaultAuthenticated ?? _options.Value.IsDefaultAuthenticated,
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

    public async Task<IEasySocket?> GetInstance(EasySocketTypeCache cache, WebSocketManager websockets, string roomId, string clientId)
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

        return easySocket;
    }
}