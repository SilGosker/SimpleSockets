using EasySockets.Middleware;
using EasySockets.Services.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
	/// <returns>A <see cref="EasySocketBuilder" /> To further configure the EasySockets.</returns>
	public static EasySocketBuilder UseEasySockets(this IApplicationBuilder app)
	{
        var options = app.ApplicationServices.GetRequiredService<IOptions<EasySocketGlobalOptions>>();

		app.UseWebSockets(options.Value.WebSocketOptions);
		app.UseMiddleware<SocketMiddleware>();

        var easySocketTypeHolder = app.ApplicationServices.GetRequiredService<EasySocketTypeHolder>();
        return new EasySocketBuilder(easySocketTypeHolder);
    }
}