using Microsoft.AspNetCore.Builder;
using SimpleSockets.Middleware;

namespace SimpleSockets.Extensions;

public static class AppBuilderExtensions
{
    public static void UseSimpleSockets(this IApplicationBuilder app)
    {
        app.UseWebSockets(new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromSeconds(10)
        });
        app.UseMiddleware<SocketMiddleware>();
        
    }

    public static IApplicationBuilder AddSimpleSocket<T>(this IApplicationBuilder app, string url) where T : SimpleSocket
    {
        SocketMiddleware.SetBehavior<T>(url);
        return app;
    }
}