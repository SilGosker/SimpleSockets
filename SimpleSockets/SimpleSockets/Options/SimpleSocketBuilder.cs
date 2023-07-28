using Microsoft.AspNetCore.Builder;
using SimpleSockets.DataModels;
using SimpleSockets.Interfaces;
using SimpleSockets.Middleware;

namespace SimpleSockets.Options;

public class SimpleSocketBuilder
{
    /// <summary>
    /// Adds a SimpleSocket type without authentication to the available websocket endpoints.
    /// If you do want to use authentication, use <see cref="AddSimpleSocket(string, Type, Action{SimpleSocketOptions})"/>
    /// </summary>
    /// <typeparam name="TSimpleSocket">The type of simple socket you want to use</typeparam>
    /// <param name="url">The url that the websockets url should match</param>
    /// <returns>A <see cref="SimpleSocketBuilder"/> that can further configure the simple socket behaviors</returns>
    public SimpleSocketBuilder AddSimpleSocket<TSimpleSocket>(string url)
        where TSimpleSocket : ISimpleSocket
        => AddSimpleSocket(url, typeof(TSimpleSocket), null);

    /// <summary>
    /// Adds a SimpleSocket type without authentication to the available websocket endpoints.
    /// If you do want to use authentication, use <see cref="AddSimpleSocket(string, Type, Action{SimpleSocketOptions})"/>
    /// </summary>
    /// <typeparam name="TSimpleSocket">The type of simple socket you want to use</typeparam>
    /// <param name="url">The url that the websockets url should match</param>
    /// <param name="configure">An <see cref="Action{SimpleSocketOptions}"/> to configure the given options of the specific simpleSocket</param>
    /// <returns>A <see cref="SimpleSocketBuilder"/> that can further configure the simple socket behaviors</returns>
    public SimpleSocketBuilder AddSimpleSocket<TSimpleSocket>(string url, Action<SimpleSocketOptions> configure)
        where TSimpleSocket : ISimpleSocket
        => AddSimpleSocket(url, typeof(TSimpleSocket), configure);

    private SimpleSocketBuilder AddSimpleSocket(string url, Type simpleSocketType, Action<SimpleSocketOptions>? configure)
    {
        var options = new SimpleSocketOptions();
        configure?.Invoke(options);
        SimpleSocketInstanceFactory.AddType(url, SimpleSocketTypeContainer.Create(simpleSocketType, options));
        return this;
    }
}