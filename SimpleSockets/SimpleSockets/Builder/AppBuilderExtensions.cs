using Microsoft.AspNetCore.Builder;
using EasySockets.Middleware;

namespace EasySockets.Builder;

/// <summary>
///     The exposed EasySockets extensions for the <see cref="IApplicationBuilder" />.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    ///     Adds the EasySockets middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application that the EasySockets middleware should be added to.</param>
    /// <param name="configure">A function to configure the provided <see cref="EasySocketMiddlewareOptions" />.</param>
    /// <returns>A <see cref="EasySocketBuilder" /> To further configure the EasySockets.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static EasySocketBuilder UseEasySockets(this IApplicationBuilder app,
        Action<EasySocketMiddlewareOptions>? configure = null)
    {
        var options = new EasySocketMiddlewareOptions();
        configure?.Invoke(options);
        if (options == null)
            throw new ArgumentException($"The {nameof(configure)} method cannot make the options null",
                nameof(configure));

        app.UseWebSockets(options.WebSocketOptions);
        app.UseMiddleware<SocketMiddleware>(options);
        return new EasySocketBuilder();
    }
}