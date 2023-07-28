using Microsoft.AspNetCore.Builder;
using SimpleSockets.Middleware;
using SimpleSockets.Options;

namespace SimpleSockets.Extensions;

public static class AppBuilderExtensions
{
    /// <summary>
    /// Adds the SimpleSockets middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application that the pipeline should be added to</param>
    /// <returns>A <see cref="SimpleSocketBuilder"/> To further configure the SimpleSockets</returns>
    public static SimpleSocketBuilder UseSimpleSockets(this IApplicationBuilder app) => UseSimpleSockets(app, null);

    /// <summary>
    /// Adds the SimpleSockets middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application that the pipeline should be added to</param>
    /// <param name="configure">A function to configure the provided <see cref="SimpleSocketMiddlewareOptions"/>.</param>
    /// <returns>A <see cref="SimpleSocketBuilder"/> To further configure the SimpleSockets</returns>
    public static SimpleSocketBuilder UseSimpleSockets(this IApplicationBuilder app, Action<SimpleSocketMiddlewareOptions>? configure)
    {
        var options = new SimpleSocketMiddlewareOptions();
        configure?.Invoke(options);
        app.UseWebSockets(options.WebSocketOptions);
        app.UseMiddleware<SocketMiddleware>(options);
        return new SimpleSocketBuilder();
    }
}