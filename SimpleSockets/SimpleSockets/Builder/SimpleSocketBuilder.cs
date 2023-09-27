using SimpleSockets.DataModels;
using SimpleSockets.Interfaces;
using SimpleSockets.Middleware;

namespace SimpleSockets.Builder;

/// <summary>
///     The class to further configure a specific SimpleSocket.
/// </summary>
public class SimpleSocketBuilder
{
    /// <summary>
    ///     Adds a simpleSocket type with configuration to the available websocket endpoints.
    /// </summary>
    /// <typeparam name="TSimpleSocket">
    ///     <para>
    ///         The simpleSocket type.
    ///     </para>
    ///     <para>
    ///         Although the type is only required to implement the <see cref="ISimpleSocket" /> interface, it is not
    ///         recommended to use that as
    ///         the type parameter. <br />
    ///         It is recommended to inherit from the <see cref="SimpleSocket" />, <see cref="EventSocket{TEvent}" /> or
    ///         <see cref="EventSocket" /> class
    ///         and use that as the type parameter, as these contain logic the middleware expects it to have (like accepting
    ///         and receiving messages through websockets).
    ///     </para>
    /// </typeparam>
    /// <param name="url">The endpoint that is made available for clients websocket requests.</param>
    /// <param name="configure">
    ///     An <see cref="Action{SimpleSocketOptions}" /> to configure the given options of the specific
    ///     simpleSocket.
    /// </param>
    /// <returns>A <see cref="SimpleSocketBuilder" /> that can further configure the simple socket behaviors.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public SimpleSocketBuilder AddSimpleSocket<TSimpleSocket>(string url, Action<SimpleSocketOptions>? configure = null)
        where TSimpleSocket : ISimpleSocket
    {
        return AddSimpleSocket(url, typeof(TSimpleSocket), configure);
    }

    private SimpleSocketBuilder AddSimpleSocket(string url, Type simpleSocketType,
        Action<SimpleSocketOptions>? configure)
    {
        if (url == null)
            throw new ArgumentNullException(nameof(url));
        var options = new SimpleSocketOptions();
        configure?.Invoke(options);
        if (options == null)
            throw new ArgumentException($"The {nameof(configure)} method cannot make the options null",
                nameof(configure));
        SimpleSocketInstanceFactory.AddType(url, SimpleSocketTypeContainer.Create(simpleSocketType, options));
        return this;
    }
}