using Microsoft.AspNetCore.Builder;
using SimpleSockets.Interfaces;
using SimpleSockets.Middleware;

namespace SimpleSockets.Extensions;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Adds the SimpleSockets middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application that the pipeline should be added to</param>
    public static IApplicationBuilder UseSimpleSockets(this IApplicationBuilder app)
    {
        app.UseWebSockets(new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromSeconds(10)
        });
        app.UseMiddleware<SocketMiddleware>();
        return app;
    }
    
    /// <summary>
    /// Adds a SimpleSocket type without authentication to the available websocket endpoints.
    /// If you do want to use authentication, use <see cref="AddSimpleSocket{TSimpleSocket, TSimpleSocketAuthenticator}"/>
    /// </summary>
    /// <typeparam name="TSimpleSocket">The type of simple socket you want to use</typeparam>
    /// <param name="app">The app that the websockets endpoint should be added to</param>
    /// <param name="url">The url that the websockets url should match</param>
    /// <returns>The <see cref="IApplicationBuilder"/> that was parsed in through the <see cref="app"/> argument </returns>
    public static IApplicationBuilder AddSimpleSocket<TSimpleSocket>(this IApplicationBuilder app, string url)
        where TSimpleSocket : ISimpleSocket
    {
        SocketMiddleware.SetBehavior<TSimpleSocket>(url);
        return app;
    }

    /// <summary>
    /// Adds a SimpleSocket type with authentication to the available websocket endpoints.
    /// </summary>
    /// <typeparam name="TSimpleSocket">The type of simple socket you want to use</typeparam>
    /// <typeparam name="TSimpleSocketAuthenticator">The type of authenticator you want to use</typeparam>
    /// <param name="app">The app that the websockets endpoint should be added to</param>
    /// <param name="url">The url that the websockets url should match</param>
    /// <returns>The <see cref="IApplicationBuilder"/> that was parsed in through the <see cref="app"/> argument </returns>
    public static IApplicationBuilder AddSimpleSocket<TSimpleSocket, TSimpleSocketAuthenticator>(this IApplicationBuilder app, string url)
        where TSimpleSocket : ISimpleSocket
        where TSimpleSocketAuthenticator : ISimpleSocketAsyncAuthenticator
    {
        SocketMiddleware.SetBehavior<TSimpleSocket, TSimpleSocketAuthenticator>(url);
        return app;
    }
}