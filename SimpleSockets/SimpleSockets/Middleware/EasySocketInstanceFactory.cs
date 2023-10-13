using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using EasySockets.Authentication;
using EasySockets.DataModels;
using EasySockets.Interfaces;

namespace EasySockets.Middleware;

internal class EasySocketInstanceFactory
{
    private static readonly Dictionary<string, EasySocketTypeContainer> EasySocketTypes = new();
        
    internal static void AddType(string url, EasySocketTypeContainer simpleSocketType)
    {
        EasySocketTypes.Add(url, simpleSocketType);
    }
        
    internal static async Task<IEasySocket?> GetAuthenticatedInstance(HttpContext context, bool isAuthenticatedByDefault, string defaultRoomId, string defaultUserId)
    {
        var simpleSocketTypeCache = EasySocketTypes.GetValueOrDefault(context.Request.Path.ToString());
        if (simpleSocketTypeCache is null) return null;

        using var scope = context.RequestServices.CreateScope();
        EasySocketAuthenticationResult authenticationResult = new(simpleSocketTypeCache.Options.IsDefaultAuthenticated ?? isAuthenticatedByDefault);

        foreach (var authenticatorType in simpleSocketTypeCache.Options.Authenticators)
        {
            var authenticator = ActivatorUtilities.CreateInstance(scope.ServiceProvider, authenticatorType);

            authenticationResult = authenticator switch
            {
                IEasySocketAsyncAuthenticator asyncAuthenticator => await asyncAuthenticator.AuthenticateAsync(
                    authenticationResult),
                IEasySocketAuthenticator syncAuthenticator =>
                    syncAuthenticator.Authenticate(authenticationResult),
                _ => authenticationResult
            };

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract : never trust your users input
            if (authenticationResult?.IsAuthenticated != true) return null;

        }

        var ws = context.WebSockets.WebSocketRequestedProtocols.Count > 0
            ? await context.WebSockets.AcceptWebSocketAsync(context.WebSockets.WebSocketRequestedProtocols[0])
            : await context.WebSockets.AcceptWebSocketAsync();

        if (ws == null) return null;
        if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, simpleSocketTypeCache.EasySocketType, ws, simpleSocketTypeCache.Options) is not IEasySocket simpleSocket)
            return null;
            
        try
        {
            simpleSocket.UserId = authenticationResult.UserId ?? defaultUserId;
            simpleSocket.RoomId = authenticationResult.RoomId ?? defaultRoomId;
        }
        catch (Exception)
        {
            throw new InvalidOperationException(
                "The UserId or RoomId should not be set in the constructor of any EasySocket");
        }
            
        return simpleSocket;
    }
}