# EasySockets

EasySockets is a powerful tool designed to simplify the process of working with websockets, especially when dealing with advanced features such as custom event binding and extracting. Whether you're a beginner looking for a straightforward way to create websockets or an expert in need of intricate functionalities, EasySockets is tailored to meet your requirements.

## Features
* Simple WebSocket Creation: Set up a websocket connection with minimal configuration.
* Custom Event Binding: Easily bind custom events to your websockets and manage event-driven programming.
* Custom Authentication/Authorization: EasySockets allows you to create your own authentication/authorization methods.

## Installation
With the Package Manager Console:
```
Install-Package EasySockets
```
With the .net CLI:
```
nuget install EasySockets
```
When using the Nuget Exporer, search for `EasySockets` and you'll be on your way

## Examples
This section will go through the process of adding EasySockets to your application.
As an example, we will be making a small chat application and expand this the more we dive deep into the rich features EasySockets has to offer.

*do note that this is will only guide you through the backend process, not the frontend. When referring to 'make a request', you should find your own implementation of doing so. This can be through the browser and connecting using the JavaScript `Websocket` class, or using an application like Postman. Also note that the websocket request type is `ws` or `wss` depending on your SSL certificate*

### Basic Usage
Lets start by adding EasySockets to your application. apply the following code:

```C#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketService();

//other dependencies that you might want to add to your DI container

var app = builder.Build();

//other tools you might want to add/configure to your pipeline.

app.UseEasySockets();
```

The `builder.Services.AddEasySocketService();` adds the `IEasySocketService` available for DI. This manages all the websocket connections, and you can manipulate those connections outside of the websocket instances. For example, you can send messages to the client in a controller, http endpoint or your own custom services.

The `app.UseEasySockets();` adds the middleware that handles authentication and accepts (or declines) the websocket connection. If you want authentication based on the `HttpContext.User` property, make sure that you call this method **after** calling the `app.UseAuthentication()` and `app.UseAuthorization()`.

This on its own doesn't do a whole lot. Why? Because we haven't configured anything.
There is no behavior added to the pipeline thus every websocket request will result in an `403` error.

So lets create a behavior that allows us to connect to the server:
```C#
public class ChatSocket : EasySocket
{
    public ChatSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {
    }

    public override Task OnMessage(string message)
    {
        return Task.CompletedTask;
    }
}
```
This is the bare minimum behavior. The long constructor is required to make the code compile and work. The `OnMessage` method is invoked whenever the server receives a message from the client.

But in our chat application, we want other clients that are connected to the server to receive the client's message!

This can be achieved fairly simply:
```C#
public class ChatSocket : EasySocket
{
    public ChatSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {
    }

    public override Task OnMessage(string message)
    {
        return BroadCast(message);
    }
}
```
The `BroadCast` method is part of the EasySocket class. It has a few overloads, which we will discuss later on.

When `BroadCast` is called, the passed string will be sent to all connected clients specified by a requirement. How these requirements work will be discussed later on.

The default requirement is matching the RoomId. More clarifications about how these requirements work can be found later on.

Of course we can send an extra message when the client is connected or leaves the server:
```C#
public class ChatSocket : EasySocket
{
    public ChatSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {
    }

    public override Task OnMessage(string message)
    {
        return BroadCast(message);
    }

    public override Task OnConnect()
    {
        return BroadCast("Connected!");
    }

    public override Task OnDisconnect()
    {
        return BroadCast("Left!");
    }
}
```
The `OnConnect` method is invoked when the client is succesfully connected to the server,
while the `OnDisconnect` method is invoked when the client loses connection with the server.

For the sake of simplicity in this tutorial, we won't use these in the current application. Just know these are there.

Now that we have created our behavior, lets add that to the pipeline:
```C#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketService();

//other dependencies that you might want to add to your DI container

var app = builder.Build();

//other tools you might want to add/configure to your pipeline.

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat");
```
The following piece of code will add the `ChatSocket` type to the `/chat` url. Whenever a websocket request is made to `/chat`, a `ChatSocket` instance will be created and the `OnConnect` method will be invoked. Whenever the client sends a message to the server, the `OnMessage` method will be invoked. When the client disconnects from the server, the `OnDisconnect` method will be invoked.

Now if you run the application and make a websocket request to `/chat`, you should connect to the server. If you make a second request and send a message, all other clients will receive that message!

### Authentication and Authorization
Cool, you have built your very simple backend chat-application!
Next up is authentication and authorization. The following topics are discussed:
* Authenticating a client.
* Authorization.
* How Rooms work.
* Dividing clients in rooms.
* Basic usage of the `IEasySocketService`.

*This part uses the code from the previous chapter.*

We currently have a problem with our chat application: Each and every user is connected to the same room.
This causes each user to receive the messages of each other user. Not very practical for our 'privacy-focussed' chat application.

This can be solved using 2 methods:
1. Setting up an authenticator for our ChatSocket 
2. changing the default RoomId method.

First we'll discuss setting up an authenticator, since this is what you'll do the most as it is best practice.

To set up an authenticator, create a class that implements the `IEasySocketAuthenticator` or `IEasySocketAsyncAuthenticator`.
```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;

namespace MyApplication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult previousAuthenticationResult)
    {
        return true;
    }
}
```
In this example, we have implemented the `IEasySocketAuthenticator`. The `IEasySocketAsyncAuthenticator` can be used if async operations are necessary:
```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;

namespace MyApplication;

public class ChatAuthenticator : IEasySocketAsyncAuthenticator
{
    public Task<EasySocketAuthenticationResult> AuthenticateAsync(EasySocketAuthenticationResult previousAuthenticationResult)
    {
        return Task.FromResult(new EasySocketAuthenticationResult(true));
    }
}
```

Lets break down what exactly the code above does:
1. These authenticators are created and the `Authenticate` or `AuthenticateAsync` methods are invoked for each websocket request that has a matching type to the requests url.
In our case this is `/chat` to the `ChatSocket`. If we where to connect to `/hello-world`, these instances wouldn't be created.
2. The `Authenticate` and `AuthenticateAsync` methods both have the `previousAuthenticationResult` argument. This is what the previous authenticator returned.
You can add as many authenticators as you'd like to this pipeline, so this can be important as your authenticators scale. We'll discuss how later on.
3. The return result is `true` and `Task.FromResult(new EasySocketAuthenticationResult(true))`. These both do the same, but for sake of laziness i added an implicit cast from a boolean.
4. The boolean value determines whether the websocket is allowed to connect to the server or not. If at any point the return value would be `false`, the pipeline would stop invoking new authenticators and the request would result in a `403 - Unauthorized` status code.

Now that we know this, we can do some fun stuff! For example, lets inject the `IEasySocketService` and have a look at what it can do!

*You can inject your own services into the authenticator if needed*
```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;
using EasySockets.Services;

namespace MyApplication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    private readonly IEasySocketService _easySocketService;

    public ChatAuthenticator(IEasySocketService easySocketService)
    {
        _easySocketService = easySocketService;
    }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult previousAuthenticationResult)
    {
        if (_easySocketService.Count() % 2 == 0) //if the amount of sockets are dividable by 2
        {
            return new(true, "EvenRoom"); //send it to the even room
        }
        return new(true, "OddRoom"); //the amount of sockets aren't dividable by 2, send the socket to the odd room
    }
}
```
This piece of code divides all sockets that are connecting to `/chat` into 2 rooms: `EvenRoom` and `OddRoom`. Every first, third, fifth socket will be connected to the `OddRoom`, while all others will be connected to the `EvenRoom`.
Of course this isn't secure. If you made a second request to the same url you'll be sent to the other room.
The responsibility of authentication/authorization is up to the developer.

Now that our authenticator is ready, lets add it to the authentication pipeline:
```C#
// ... other build stuff

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat", options =>
    {
        options.AddAuthenticator<ChatAuthenticator>();
    });
```
If you re-run the application and make 2 websocket requests to `/chat` and send a message, you'll notice that none of them are receiving the other ones messages.
When making a third websocket request, the first connection will receive your messages, since you both are in room with the `EvenRoom` Id.

You can use the `UserId` and `RoomId` property so that when the user is connected, he'll receive a welcome message:
```C#
using System.Net.WebSockets;
using EasySockets;
using EasySockets.Builder;

namespace MyApplication;


public class ChatSocket : EasySocket
{
    public ChatSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {
    }

    public override Task OnMessage(string message)
    {
        return BroadCast(message);
    }

    public override Task OnConnect()
    {
        // use the RoomId property to welcome the user to the current room
        return BroadCast($"Welcome {UserId}. You are currently in room '{RoomId}'");
    }
}
```

You can also use the `RoomId` and `UserId` outside of the `EasySocket` instance using the `IEasySocketService`:
```C#
// ... other build stuff

app.MapGet("/count", (IEasySocketService easySocketService) =>
{
    var evenMembers = easySocketService.Count("EvenRoom"); //count all connected clients in the room with id 'EvenRoom'
    var oddMembers = easySocketService.Count("OddRoom"); //count all connected clients in the room with id 'OddRoom'
    return $"There are {evenMembers} in team Even, and {oddMembers} in team Odd";
});

app.MapGet("/hasmember/{roomId}/{userId}", (IEasySocketService easySocketService, string roomId, string userId) =>
{
    // check if a client is connected to the server in the specified room, with the specified userId
    return easySocketService.Any(roomId, userId)
        ? $"{userId} is connected to the server in room '{roomId}'"
        : $"No user found with userId '{userId}' in room '{roomId}'";
});
```

However, this introduces a problem. If you had a sneak peak using a debugger, you may have noticed that the `UserId` property is always a random guid. You may want to control this behavior. You can do so by changing the constructor in any `Authenticator`:

```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;
using EasySockets.Services;

namespace MyApplication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    private readonly IEasySocketService _easySocketService;

    public ChatAuthenticator(IEasySocketService easySocketService)
    {
        _easySocketService = easySocketService;
    }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult previousAuthenticationResult)
    {
        int connectedClients = _easySocketService.Count();
        if (connectedClients % 2 == 0)
        {
            // note the UserId must be unique!
            return new(true, "EvenRoom", connectedClients.ToString());
        }
        // note the UserId must be unique!
        return new(true, "OddRoom", connectedClients.ToString());
    }
}

```
The added argument in the constructor will change the value of the `UserId` property to the given value. In this case, the `UserId` would become the total amount of clients connected to the server. We can change this behavior so that instead of the total amount of clients connected to the server, we can assign the `UserId` to the total amount of clients connected to the room instead:
```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;
using EasySockets.Services;

namespace MyApplication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    private readonly IEasySocketService _easySocketService;

    public ChatAuthenticator(IEasySocketService easySocketService)
    {
        _easySocketService = easySocketService;
    }

    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult previousAuthenticationResult)
    {
        int connectedClients = _easySocketService.Count();
        if (connectedClients % 2 == 0)
        {
            roomMembers = _easySocketService.Count("EvenRoom");
            return new(true, "EvenRoom", roomMembers.ToString());
        }
        roomMembers = _easySocketService.Count("EvenRoom");
        return new(true, "OddRoom", roomMembers.ToString());
    }
}
```
*It is recommended (but not required) that the UserId is always unique. When manipulating any connections through the `IEasySocketService`, it mostly looks for a single connection. Creating duplicate userId's causes only a single connection to be interacted with.*

But how can this be helpful? Because now that we know these values, we can manipulate the websockets outside of the instances:

```C#
app.MapGet("/only-admins/disconnect-team-even", (IEasySocketService easySocketService) =>
{
    easySocketService.ForceLeave("EvenRoom");
});

app.MapGet("/only-admins/disconnect-last-member", (IEasySocketService easySocketService) =>
{
    var membersInEvenRoom = service.Count("EvenRoom");

    easySocketService.ForceLeave("EvenRoom", membersInEvenRoom.ToString());
});
```
In the endpoint `/only-admins/disconnect-team-even`, given that we know that a room is present with the RoomId 'EvenRoom', we could disconnect all clients connected to that room.

In the endpoint `/only-admins/disconnect-last-member`, given that we know that the last user joined has that number assigned to the 'UserId', we can force that specific user to disconnect from the server.

*Note that these are all examples. There should not be an endpoint that disconnects a room or user from the server. These endpoints purely serve as examples so you can understand the code, play with it and implement it in your own applications. In a real-world scenario, you may want to assign something from the database to the RoomId or UserId of a connection.*