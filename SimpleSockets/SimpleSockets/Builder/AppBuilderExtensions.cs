using Microsoft.AspNetCore.Builder;
using SimpleSockets.Middleware;

namespace SimpleSockets.Builder;

/// <summary>
///     The exposed SimpleSockets extensions for the <see cref="IApplicationBuilder" />.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    ///     Adds the SimpleSockets middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application that the SimpleSockets middleware should be added to.</param>
    /// <param name="configure">A function to configure the provided <see cref="SimpleSocketMiddlewareOptions" />.</param>
    /// <returns>A <see cref="SimpleSocketBuilder" /> To further configure the SimpleSockets.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static SimpleSocketBuilder UseSimpleSockets(this IApplicationBuilder app,
        Action<SimpleSocketMiddlewareOptions>? configure = null)
    {
        var options = new SimpleSocketMiddlewareOptions();
        configure?.Invoke(options);
        if (options == null)
            throw new ArgumentException($"The {nameof(configure)} method cannot make the options null",
                nameof(configure));

        app.UseWebSockets(options.WebSocketOptions);
        app.UseMiddleware<SocketMiddleware>(options);
        return new SimpleSocketBuilder();
    }
}