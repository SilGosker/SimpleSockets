using EasySockets.Middleware;
using EasySockets.Services.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EasySockets.Builder;

/// <summary>
///     The exposed EasySockets extensions for the <see cref="IApplicationBuilder" />.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    ///     Adds the EasySockets middleware to the pipeline. <br /> <br />
    ///     <b>
    ///         Make sure the <c>app.UseWebSocket(WebSocketOptions options = null)</c> is called before this method.
    ///         Otherwise no websocket connection will be able to connect.
    ///     </b>
    /// </summary>
    /// <param name="app">The application that the EasySockets middleware should be added to.</param>
    /// <returns>A <see cref="EasySocketBuilder" /> To further configure the EasySockets.</returns>
    public static EasySocketBuilder UseEasySockets(this IApplicationBuilder app)
    {
        app.UseMiddleware<SocketMiddleware>();

        var easySocketTypeHolder = app.ApplicationServices.GetRequiredService<EasySocketTypeHolder>();
        return new EasySocketBuilder(easySocketTypeHolder);
    }
}