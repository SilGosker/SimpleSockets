using EasySockets.Builder;
using TestApplication.Websockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat", options =>
    {
        options.IsDefaultAuthenticated = true;
    });

app.Run();