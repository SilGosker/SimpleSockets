# EasySockets

EasySockets is a powerful tool designed to simplify the process of working with websockets, especially when dealing with advanced features such as custom event binding and extracting, custom authentication and authorization and websocket manipulation wherever you want in your code. Whether you're a beginner looking for a straightforward way to create websockets or an expert in need of intricate functionalities, EasySockets is tailored to meet your requirements.

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
When using the Nuget Exporer, search for `EasySockets` and install the package.

## Examples
This section will go through the process of adding EasySockets to your application.
As an example, we will be making a small chat application and expand this the more we dive deep into the rich features EasySockets has to offer.

*do note that this is will only guide you through the backend process, not the frontend. When referring to 'make a request', you should find your own implementation of doing so. This can be through the browser and connecting using the JavaScript `Websocket` class, or using an application like Postman. Also note that the websocket protocol  is `ws` or `wss` depending on your SSL certificate*

### Basic Usage
Lets start by adding EasySockets to your application. apply the following code:

```C#
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

//other dependencies that you might want to add to your DI container

var app = builder.Build();

app.UseHttpsRedirection();
//other tools you might want to add/configure to your pipeline.

app.UseEasySockets();
```

The `builder.Services.AddEasySocketServicesm();` adds the `IEasySocketService` available for DI. This manages all the websocket connections. You can manipulate those connections outside of the websocket instances. For example, you can send messages to the client in a controller or custom services.

The `app.UseEasySockets();` adds the middleware that handles authentication and accepts (or declines) a client. If you want authentication based on the `HttpContext.User` property, make sure that you call this method **after** calling the `app.UseAuthentication()`; and `app.UseAuthorization();`.

This on its own doesn't do a whole lot. Why? Because no behavior is added to the pipeline. Every websocket request will result in an `403 - Forbidden` status code.

So lets create a behavior that allows us to connect to the server:
```C#
using System.Net.WebSockets;
using EasySockets;
using EasySockets.Builder;

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
using System.Net.WebSockets;
using EasySockets;
using EasySockets.Builder;

public class ChatSocket : EasySocket
{
    public ChatSocket(WebSocket webSocket, EasySocketOptions options) : base(webSocket, options)
    {
    }

    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }
}
```
Instead of returning `Task.CompletedTask`, we return the task returned by the build-in `BroadCast` method. The `BroadCast` method has a few overloads, which will be discussed later on.

When `BroadCast` is called, the passed string will be sent to all other connected clients. The [Authentication and Authorization](### Authentication and Authorization) section will explain more about how this works behind the scenes.

Of course we can send an extra message when the client connects or leaves the server:
```C#
using System.Net.WebSockets;
using EasySockets;
using EasySockets.Builder;

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
The `OnConnect` method is invoked when the client is successfully connected to the server while the `OnDisconnect` method is invoked when the client loses connection with the server.

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
* Rooms functionality.
* Dividing clients in (custom) rooms.
* UserId customization.

*This part uses the code from the previous chapter.*
#### Rooms
A room works how real-life rooms work. If you and another person are having a conversation in the kitchen, people in the bedroom won't hear you and the other way around. This principle works the same in EasySockets. People in Room `ABC` won't get to hear what is broadcasted from room `XYZ`.

We currently have a problem with our chat application: Each and every user is connected to the same room. If we want more privacy built into our chat application, we should fix this. 

This can be solved using 2 methods:
1. Setting up an authenticator for our ChatSocket.
2. manipulating the default RoomId.

First we'll discuss setting up an authenticator, since this is what you'll do the most as it is the best practice when using EasySockets.

#### Authenticators
To set up an authenticator, create a class that implements the `IEasySocketAuthenticator` or `IEasySocketAsyncAuthenticator`.
```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult, HttpContext context)
    {
        return true;
    }
}
```

```C#
using EasySockets.Authentication;
using EasySockets.Interfaces;

public class ChatAuthenticator : IEasySocketAsyncAuthenticator
{
    public Task<EasySocketAuthenticationResult> AuthenticateAsync(EasySocketAuthenticationResult currentAuthenticationResult, HttpContext context)
    {
        return Task.FromResult(new EasySocketAuthenticationResult(true));
    }
}
```
The `IEasySocketAsyncAuthenticator` can be used if async operations are necessary.

Lets break down exactly how the code above behaves:
1. These authenticators are created and the `Authenticate` or `AuthenticateAsync` methods are invoked for each websocket request that has a matching type to the requests url.
   *In our case this is `/chat` to the `ChatSocket`. If we where to connect to `/hello-world`, these instances wouldn't be created.*
2. The `Authenticate` and `AuthenticateAsync` methods both have the `currentAuthenticationResult` argument. This is the default authentication result or what the last authenticator returned.
   *Note: You can configure what the default authentication result is and you can add as many custom authenticators as you'd like to this pipeline so this is important as your authenticators scale. We'll discuss both of these topics later on in this tutorial.*
3. The return result `true` and `Task.FromResult(new EasySocketAuthenticationResult(true))` both do the same. A implicit converter can be found on the EasySocketAuthenticationResult so that the value `true` does the same as `new EasySocketAuthenticationResult(true)`.
   *The boolean value determines whether the websocket is allowed to connect to the server or not. If at any point the return value would be `false`, the pipeline would stop creating new authenticator instances and the request would result in a `403 - Unauthorized` status code.*
4. The `HttpContext` is added so you can get parameters from the current request without having to inject the `IHttpContextAccessor`.
5. If needed, you can inject your own services through dependency injection.

In this example, we will use the `IEasySocketAuthenticator` since we don't need asynchronous operations.

Let's build our authentication system. In this example, we use a `slug` query parameter that contains a string. If no slug parameter is found, we return `false`, indicating that the websocket is not allowed to connect to the server. Otherwise, we return a successful authentication result with the slug as the room identifier:
```C#
using EasySockets.Authentication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("slug", out var slug))
        {
            return false;
        }

        return new EasySocketAuthenticationResult(true, slug);
    }
}
```
``
Now that our authenticator is ready, lets add it to the authentication pipeline:
```C#
// ... other build stuff

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat", options =>
    {
        options.AddAuthenticator<ChatAuthenticator>();
    });
```

Now if you run the application and connect 2 clients to `/chat?slug=room0` and send a message, the other client will receive that message. But if you connect a third client to `/chat?slug=room1` and send a message, no other clients will receive that message. And finally if you would connect to `/chat`, you would receive the `403 - Unauthorized` status code.

Let's build in a welcoming message. When the user successfully connects to the server, we want the server to send back a welcoming message:
```C#
using System.Net.WebSockets;
using EasySockets;
using EasySockets.Builder;

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
        return BroadCast(BroadCastFilter.EqualRoomId, $"Welcome {UserId}. You are currently in room '{RoomId}'");
    }
}
```
A different overload of the `BroadCast` method is used to specify the requirement that other clients should match to receive the message. The default requirement is a matching room, ignoring the instance that called the `BroadCast` method. In our case, we want this instance to also receive the welcoming message, so we only want the matching room requirement. This will include the client that just connected.

You can not only manipulate the room the user comes into, you can also manipulate the unique identifier the client is assigned. The default is a random guid, but you can manipulate this to your needs:
```C#
using EasySockets.Authentication;
using EasySockets.Services;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("slug", out var slug))
        {
            return false;
        }

        return new EasySocketAuthenticationResult(true, slug, Random.Shared.Next().ToString());
    }
}
```
In this example, we set the unique identifier to a random number instead of a random guid. You'll learn why this is important in the next chapter: **The IEasySocketService**

#### Authentication through configuration
You can authenticate clients through configuration as well. This allows you to set up default authentication and authorization. Let's start with authentication:
```C#
using Authentication.Websockets;
using EasySockets.Builder;
using EasySockets.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketService();

// Add services to the container.

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets(options =>
	{
		options.IsDefaultAuthenticated = true;
	})
	.AddEasySocket<ChatSocket>("/chat");

app.Run();
```
Since the `IsDefaultAuthenticated` property works globally, this causes **every single** websocket connection to successfully authenticate to the server. Since we don't want this, we can configure specifically for our `ChatSocket` that we don't want it to authenticate easily:
```C#
using Authentication.Websockets;
using EasySockets.Builder;
using EasySockets.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketService();

// Add services to the container.

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets(options =>
	{
		options.IsDefaultAuthenticated = false;
	})
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.IsDefaultAuthenticated = true;
	});

app.Run();
```

We set the global `IsDefaultAuthenticated` to `false`, and the `IsDefaultAuthenticated` for the specific url to `true`. This causes every websocket to the `/chat` endpoint to automatically be successfully authenticated to the server. For authentication, we can't configure anything more. We can't add any custom logic as we need authenticators to do this.

That being said, we can change the default authorization, meaning the dividing of rooms and setting the unique identifier for the client:
```C#
using Authentication.Websockets;
using EasySockets.Builder;
using EasySockets.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketService();

// Add services to the container.

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets(options =>
	{
		options.GetDefaultRoomId = context => context.Request.Query["slug"];
		options.GetDefaultUserId = context => Random.Shared.Next().ToString();
	})
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.IsDefaultAuthenticated = true;
	});

app.Run();
```
We have changed the `GetDefaultRoomId` and `GetDefaultUserId` to a method that returns what otherwise the authenticators would return.
*Note that if a client would connect to `/chat` without the `slug` query parameter, the system would throw an exception. **The GetDefaultRoomId and  GetDefaultUserId should never return `null`.** If they would **and** no RoomId or UserId is specified in the last `EasysocketAuthenticationResult`, the system would throw an exception **after** the websocket is accepted, causing an 'unclean' closing status of the websocket.*
### Manipulating EasySockets with the IEasySocketService

The `IEasySocketService` allows you to manipulate the websocket connections outside of the EasySocket instances, meaning in any other controller, mapped endpoint or custom service. This allows for dynamic behaviors to be set up.

In this tutorial, we'll discuss the following topics:
1. Sending messages back to the client.
2. Disconnecting clients from the server.
3. Checking connection states.

#### Sending messages to the client
