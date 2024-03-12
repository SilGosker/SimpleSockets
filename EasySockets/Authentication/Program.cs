using Authentication.Authenticators;
using Authentication.Websockets;
using EasySockets.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

// Add services to the container.

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});

app.Run();
