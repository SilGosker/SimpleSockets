using CustomEventTypes.Sockets;
using EasySockets.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat");

app.Run();