using Authentication.Websockets;
using EasySockets.Builder;
using EasySockets.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketService();

// Add services to the container.

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});

app.Run();
