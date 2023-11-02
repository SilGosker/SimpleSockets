using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EasySockets.DataModels;

namespace EasySockets.Authentication;

internal class EasySocketInstanceFactory
{
    private static readonly Dictionary<string, EasySocketTypeCache> EasySocketTypes = new();

    internal static void AddType(string url, EasySocketTypeCache simpleSocketType)
    {
        if (EasySocketTypes.ContainsKey(url)) throw new InvalidOperationException($"Url '{url}' Cannot be added twice");
        EasySocketTypes.Add(url, simpleSocketType);
    }

    internal static async Task<IEasySocket?> GetAuthenticatedInstance(HttpContext context, bool isAuthenticatedByDefault, string defaultRoomId, string defaultUserId)
    {
        var simpleSocketTypeCache = EasySocketTypes.GetValueOrDefault(context.Request.Path.ToString());
        if (simpleSocketTypeCache is null) return null;
        using var scope = context.RequestServices.CreateScope();
        EasySocketAuthenticationResult authenticationResult = new(simpleSocketTypeCache.Options.IsDefaultAuthenticated ?? isAuthenticatedByDefault, defaultRoomId, defaultUserId);

        foreach (var authenticatorType in simpleSocketTypeCache.Options.Authenticators)
        {
            var authenticator = ActivatorUtilities.CreateInstance(scope.ServiceProvider, authenticatorType);

            authenticationResult = authenticator switch
            {
                IEasySocketAsyncAuthenticator asyncAuthenticator => await asyncAuthenticator.AuthenticateAsync(authenticationResult, context),
                IEasySocketAuthenticator syncAuthenticator => syncAuthenticator.Authenticate(authenticationResult, context),
                _ => authenticationResult
            };

            if (authenticationResult.IsAuthenticated != true) return null;

        }

        var ws = context.WebSockets.WebSocketRequestedProtocols.Count > 0
            ? await context.WebSockets.AcceptWebSocketAsync(context.WebSockets.WebSocketRequestedProtocols[0])
            : await context.WebSockets.AcceptWebSocketAsync();

        if (ws == null) return null;

        if (ActivatorUtilities.CreateInstance(scope.ServiceProvider,
	            simpleSocketTypeCache.EasySocketType,
	            ws,
	            simpleSocketTypeCache.Options
            ) is not IEasySocket simpleSocket)
            return null;

        simpleSocket.InternalRoomId = authenticationResult.RoomId ?? defaultRoomId ?? throw new InvalidOperationException("The authenticationResult.RoomId and the default roomId should not be null after successful authentication");
        simpleSocket.InternalUserId = authenticationResult.UserId ?? defaultUserId ?? throw new InvalidOperationException("The authenticationResult.UserId and the default userId should not be null after successful authentication");
        return simpleSocket;
    }
}