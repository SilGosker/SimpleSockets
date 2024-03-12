using EasySocketBasicChat.Sockets;
using EasySockets.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices(e =>
{
    e.IsDefaultAuthenticated = true;
});

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat");

app.Run();