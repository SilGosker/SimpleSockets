using EasySocketAuthenticatedChat.Authenticators;
using EasySocketBasicChat.Sockets;
using EasySockets.Builder;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();
// Add services to the container.

var app = builder.Build();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat", options =>
    {
        options.AddAuthenticator<ChatAuthenticator>();
    });
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();