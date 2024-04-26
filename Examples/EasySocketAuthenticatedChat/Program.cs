using EasySocketAuthenticatedChat.Authenticators;
using EasySocketAuthenticatedChat.Sockets;
using EasySockets.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat", options =>
    {
        options.AddAuthenticator<TokenAuthenticator>();
        options.AddAuthenticator<ChatAuthenticator>();
    });


app.UseHttpsRedirection();

app.Run();