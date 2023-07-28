using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SimpleSockets.Authentication;
using SimpleSockets.DataModels;
using SimpleSockets.Interfaces;

namespace SimpleSockets.Middleware
{
    internal class SimpleSocketInstanceFactory
    {
        private static readonly Dictionary<string, SimpleSocketTypeContainer> SimpleSocketTypes = new();
        
        internal static void AddType(string url, SimpleSocketTypeContainer simpleSocketType)
        {
            SimpleSocketTypes.Add(url, simpleSocketType);
        }
        
        internal static async Task<ISimpleSocket?> GetInstanceAndAuthenticate(HttpContext context, bool isAuthenticatedByDefault, string defaultRoomId, string defaultUserId)
        {
            var simpleSocketTypeCache = SimpleSocketTypes.GetValueOrDefault(context.Request.Path.ToString());
            if (simpleSocketTypeCache is null) return null;

            using var scope = context.RequestServices.CreateScope();
            SimpleSocketAuthenticationResult authenticationResult = new(simpleSocketTypeCache.Options.IsDefaultAuthenticated ?? isAuthenticatedByDefault);

            foreach (var authenticatorType in simpleSocketTypeCache.Options.Authenticators)
            {
                var authenticator = ActivatorUtilities.CreateInstance(scope.ServiceProvider, authenticatorType);

                authenticationResult = authenticator switch
                {
                    ISimpleSocketAsyncAuthenticator asyncAuthenticator => await asyncAuthenticator.AuthenticateAsync(
                        authenticationResult),
                    ISimpleSocketAuthenticator syncAuthenticator =>
                        syncAuthenticator.Authenticate(authenticationResult),
                    _ => authenticationResult
                };

                //always expect the worst from your users
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                if (authenticationResult?.IsAuthenticated != true) return null;

            }

            var ws = context.WebSockets.WebSocketRequestedProtocols.Count > 0
                ? await context.WebSockets.AcceptWebSocketAsync(context.WebSockets.WebSocketRequestedProtocols[0])
                : await context.WebSockets.AcceptWebSocketAsync();

            if (ws == null) return null;
            if (ActivatorUtilities.CreateInstance(scope.ServiceProvider, simpleSocketTypeCache.SimpleSocketType, ws, simpleSocketTypeCache.Options) is not ISimpleSocket simpleSocket)
                return null;
            
            try
            {
                simpleSocket.UserId = authenticationResult.UserId ?? defaultUserId;
                simpleSocket.RoomId = authenticationResult.RoomId ?? defaultRoomId;
            }
            catch (Exception)
            {
                throw new InvalidOperationException(
                    "The UserId or RoomId should not be set in the constructor of any SimpleSocket");
            }
            
            return simpleSocket;
        }
    }
}