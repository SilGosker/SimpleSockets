using Microsoft.AspNetCore.Builder;
using SimpleSockets.Interfaces;
using SimpleSockets.Middleware;

namespace SimpleSockets.Options;

public class SimpleSocketBuilder
{
    private readonly IApplicationBuilder _app;

    public SimpleSocketBuilder(IApplicationBuilder app)
    {
        _app = app;
    }

    /// <summary>
    /// Adds a SimpleSocket type without authentication to the available websocket endpoints.
    /// If you do want to use authentication, use <see cref="AddSimpleSocket{TSimpleSocket, TSimpleSocketAuthenticator}"/>
    /// </summary>
    /// <typeparam name="TSimpleSocket">The type of simple socket you want to use</typeparam>
    /// <param name="app">The app that the websockets endpoint should be added to</param>
    /// <param name="url">The url that the websockets url should match</param>
    /// <returns>The <see cref="IApplicationBuilder"/> that was parsed in through the <see cref="app"/> argument </returns>
    public SimpleSocketBuilder AddSimpleSocket<TSimpleSocket>(string url)
        where TSimpleSocket : ISimpleSocket
        => AddSimpleSocket(url, typeof(TSimpleSocket), null);

    /// <summary>
    /// Adds a SimpleSocket type without authentication to the available websocket endpoints.
    /// If you do want to use authentication, use <see cref="AddSimpleSocket{TSimpleSocket, TSimpleSocketAuthenticator}"/>
    /// </summary>
    /// <typeparam name="TSimpleSocket">The type of simple socket you want to use</typeparam>
    /// <param name="app">The app that the websockets endpoint should be added to</param>
    /// <param name="url">The url that the websockets url should match</param>
    /// <param name="configure">An <see cref="Action{SimpleSocketOptions}"/> to configure the given options of the specific simpleSocket</param>
    /// <returns>The <see cref="IApplicationBuilder"/> that was parsed in through the <see cref="app"/> argument </returns>
    public SimpleSocketBuilder AddSimpleSocket<TSimpleSocket>(string url, Action<SimpleSocketOptions> configure)
        where TSimpleSocket : ISimpleSocket
        => AddSimpleSocket(url, typeof(TSimpleSocket), configure);

    private SimpleSocketBuilder AddSimpleSocket(string url, Type simpleSocketType, Action<SimpleSocketOptions>? configure)
    {
        var options = new SimpleSocketOptions();
        configure?.Invoke(options);
        SocketMiddleware.SetBehavior(url, simpleSocketType, options);
        return this;
    }
}