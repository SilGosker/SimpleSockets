# EasySockets documentation
EasySockets is a powerful tool designed to simplify the process of working with websockets, especially when dealing with advanced features such as custom event binding and extracting, custom authentication on connection level and websocket manipulation in the DI. Whether you're a beginner looking for a straightforward way to create websocket applications or an expert in need of intricate functionalities, EasySockets is tailored to meet your requirements.

## Features
* Simple WebSocket Creation: Set up a websocket connection with minimal configuration.
* Custom Event Binding: Easily bind custom events to your websockets and manage event-driven programming.
* Custom Authentication on connection level: EasySockets allows you to create your own authentication/authorization methods per individual websocket connection.

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

// Add services to the container.

var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.

app.UseEasySockets();
```

The `builder.Services.AddEasySocketServices();` adds the `IEasySocketService` available for DI. This manages all the websocket connections. You can manipulate those connections outside of the websocket instances. For example, you can send messages to the client in a controller or custom services. This is discussed later on in the tutorial.

The `app.UseEasySockets();` adds the middleware that handles authentication and accepts (or declines) a client. If you want authentication based on the `HttpContext.User` property, make sure that you call this method **after** calling the `app.UseAuthentication()`; and `app.UseAuthorization();` methods.

This on its own doesn't do a whole lot. Why? Because nothing is configured yet. Every websocket request will fail its handshake protocol.

So lets create a behavior that allows us to connect to the server:
```C#
using EasySockets;

public class ChatSocket : EasySocket
{
    public override Task OnMessage(string message)
    {
	    return Task.CompletedTask;
    }
}
```
This is the bare minimum behavior. The overridden `OnMessage` method  is required to make the code compile. The `OnMessage` method is invoked whenever the server receives a message from the client.

*Note: You can inject your custom services into our `ChatSocket` class using DI. You are allowed to inject any registered service (including transient) into this class, however this is not recommended for services whose lifetime is considered 'short' like a database connection. A websocket connection can last however long you want. If you do need transient services, inject the `IServiceScopeFactory` and obtain the required service with a custom scope.*

At the moment, we simply return a completed task. But in our chat application, we want other clients that are connected to the server to receive the client's message!

This can be achieved fairly simply:
```C#
using EasySockets;

public class ChatSocket : EasySocket
{

    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }
}
```
Instead of returning `Task.CompletedTask`, we return the task returned by the build-in `Broadcast` method. When `Broadcast` is called, the passed string will be sent to all other connected clients. The [Authentication and Authorization](#auth) section will explain more about how this works behind the scenes.

Of course we can send an extra message when the client connects or leaves the server:
```C#
using EasySockets;

public class ChatSocket : EasySocket
{
    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }

    public override Task OnConnect()
    {
        return Broadcast("Connected!");
    }

    public override Task OnDisconnect()
    {
        return Broadcast("Left!");
    }
}
```
The `OnConnect` method is invoked after the client is successfully connected to the server while the `OnDisconnect` method is invoked before the websocket is closed by the server. This means that if the server would disconnect the websocket, the `OnDisconnect` method will be invoked first, then the websocket connection would be closed. The `SendToClientAsync(string)` method is still available, meaning you can still send messages back to the client. However, if the client closes the connection, the `SendToClientAsync(string)` method wouldn't do anything. You can check the connection state using the `IsConnected()` method:
```C#
using EasySockets;

public class ChatSocket : EasySocket
{
    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }

    public override Task OnConnect()
    {
        return Broadcast("Connected!");
    }

    public override Task OnDisconnect()
    {
	    if (IsConnected())
	    {
		    return Broadcast("Left!");
	    }
        return Task.CompletedTask;
    }
}
```

For the sake of simplicity in this section of the tutorial, we won't use these now. Just know that they are there.

Now that we have created our behavior, lets add that to the pipeline:
```C#
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
```
The following piece of code will add the `ChatSocket` type to the `/chat` url. 

Whenever a websocket request is made to `/chat`, a `ChatSocket` instance will be created and the `OnConnect` method will be invoked. Whenever the client sends a message to the server, the `OnMessage` method will be invoked. When the client disconnects from the server, the `OnDisconnect` method will be invoked.

Now if you run the application and make a websocket request to `/chat`, you should connect to the server. If you make a second (and third, fourth en so on) request and send a message, all other clients will receive that message!

*Note: If at any point during the websocket connection any exception is thrown, the websocket connection will close and result in a `500 - Internal Server Error` status code.*
### Authentication and Authorization <a name="auth" />
Cool, you have built your very simple backend chat-application!
Next up is authentication and authorization. The following topics are discussed:
* Rooms functionality.
* Authenticating a client.
* Dividing clients in (customizable) rooms.
* Client identifier customization.

*Note: This part uses the code from the previous chapter.*
#### Rooms
A room works how real-life rooms work. If you and another person are having a conversation in the kitchen, people in the bedroom won't hear you and the other way around. This principle works the same in EasySockets. People in room `ABC` won't get to hear what is broadcasted from room `XYZ`.

We currently have a problem with our chat application: By default, each and every user is connected to the same room (room `__0`). If we want more privacy built into our chat application, we should fix this. 

This can be solved using 2 methods:
1. Setting up an authenticator for our `ChatSocket`.
2. manipulating the default room identifier.

First we'll discuss setting up an authenticator, since this is what you'll do the most as it is the best practice when using EasySockets.

#### Authenticators
By default, an EasySocket is allowed to connect to the server unless specified otherwise. This can be specified by setting up authenticators and adding them to the easysockets configuration. To set up an authenticator, create a class that implements the `IEasySocketAuthenticator` or `IEasySocketAsyncAuthenticator`.
```C#
using EasySockets.Authentication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult,
        HttpContext context)
    {
	    return true;
    }
}
```

```C#
using EasySockets.Authentication;

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
   *Note: You can configure what the default authentication result is and you can add as many custom authenticators as you'd like to this pipeline so this is important as your authenticators scale. We'll discuss both of these topics (default authentication and authenticator scaling) later on in this section.*
3. The return result `true` and `Task.FromResult(new EasySocketAuthenticationResult(true))` both act the same. A implicit converter can be found on the `EasySocketAuthenticationResult` struct so that the value `true` does the same as `new EasySocketAuthenticationResult(true)`.
   *Note: The boolean value determines whether the websocket is allowed to connect to the server or not. If at any point the return value would be `false`, the pipeline would stop creating new authenticator instances and the request would result in a `401 - Unauthorized` status code.*
4. If needed, you can inject your own services through dependency injection.
5. The `HttpContext` is added as an parameter so you can get parameters from the current request without having to inject the `IHttpContextAccessor`.
6. If there aren't any more authenticators in the pipeline and the boolean return value is `true`, the websocket is accepted and a new instance of the `ChatSocket` is created.

In this example, we will use the `IEasySocketAuthenticator` since we don't need asynchronous operations.

Let's build our authentication system. In this example, we will use a `slug` query parameter that contains a string. If no slug parameter is found, we return `false`, indicating that the websocket is not allowed to connect to the server. Otherwise, we return a successful authentication result with the slug as the room identifier:
```C#
using EasySockets.Authentication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult, HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("slug", out var slug))
        {
            return false;
        }

        return new EasySocketAuthenticationResult(true, slug);
    }
}
```
Now that our authenticator is ready, lets add it to the authentication pipeline:
```C#
using EasySockets.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat", options =>
    {
        options.AddAuthenticator<ChatAuthenticator>();
    });


app.UseHttpsRedirection();

app.Run();
```
The `options.AddAuthenticator<ChatAuthenticator>();` will add the `ChatAuthenticator` as the first in the authentication pipeline. You can call this method as many times as you want with as many different authenticators as you want. This allows you to split logic for authenticating the client, obtaining the room identifier or obtaining the client identifier.

*Note: For asynchronous authenticators, use the `options.AddAsyncAuthenticator<TAuthenticator>();` method.*

Now if you run the application and connect 2 clients to `/chat?slug=room0` and send a message, the other client will receive that message. But if you connect a third client to `/chat?slug=room1` and send a message, no other clients will receive that message. And finally if you would connect to `/chat`, you would receive the `401 - Unauthorized` status code.

You can access the room identifier and client identifier in the EasySocket class using the `RoomId` and `ClientId` properties. To showcase this, let's build in a welcoming message. When the user successfully connects to the server, we want the server to send back a welcoming message:
```C#
using EasySockets;
using EasySockets.Enums;

public class ChatSocket : EasySocket
{
    public override Task OnMessage(string message)
    {
        return Broadcast(message);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, $"Welcome {ClientId}. You are currently in room '{RoomId}'");
    }
}
```
*Note: A different overload of the `Broadcast` method is used to specify the requirement that other clients should match the current instance's room identifier to receive the message. The default requirement is a matching room, ignoring the instance that called the `Broadcast` method. In our case, we want this instance to also receive the welcoming message, so we only want the matching room requirement. This will include the client that just connected.*

If you would make a request to `/chat?slug=room0`, the client would immediately receive his welcoming message.

You can not only manipulate the room the user comes into, you can also manipulate the unique identifier the client is assigned. The default is a random guid, but you can manipulate this to your needs:
```C#
using EasySockets.Authentication;

public class ChatAuthenticator : IEasySocketAuthenticator
{
    public EasySocketAuthenticationResult Authenticate(EasySocketAuthenticationResult currentAuthenticationResult, HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("slug", out var slug))
        {
            return false;
        }

        return new EasySocketAuthenticationResult(true, slug, Random.Shared.Next().ToString());
    }
}
```
In this example, we set the unique identifier to a random number instead of a random guid. You'll learn why this is important later in the tutorial.

#### Identification through configuration
We can change the default identification, meaning obtaining the room and client identifiers for any client:
```C#
using System.Text;
using EasySocketEvents.Sockets;
using EasySockets.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices(options =>
{
    options.GetDefaultRoomId = context => context.Request.Query["slug"];
    options.GetDefaultClientId = _ => Random.Shared.Next().ToString();
});

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseEasySockets()
    .AddEasySocket<ChatSocket>("/chat");

app.Run();
```
We have changed the `GetDefaultRoomId` and `GetDefaultUserId` to a function that returns what would otherwise be configured by the authenticators.
*Note that if a client would connect to `/chat` without the `slug` query parameter, the system would throw an exception. **The GetDefaultRoomId and  GetDefaultUserId should never return `null`.** If they would **and** no RoomId or ClientId is specified in the last `EasysocketAuthenticationResult`, EasySockets will throw an exception **after** the websocket is accepted, causing an 'unclean' closing status of the websocket.*
### Manipulating EasySockets with the IEasySocketService
The `IEasySocketService` allows you to manipulate the websocket connections outside of the EasySocket instances, meaning in any other controller, mapped endpoint or custom service. This allows for dynamic behaviors to be set up.

In this tutorial, we'll discuss the following topics:
1. Sending messages back to the client.
2. Sending messages to all clients in a room.
3. Disconnecting clients from the server.
4. Checking connection states.

*Note: This part uses the code from the previous chapter.*

We will be using the `MapGet` method in the code examples below. This is done to showcase the feature. Note that the `IEasySocketService` is available in the DI container, meaning you can also use it in controllers, custom services and other places that use DI. 
#### Sending messages to the client
Sending messages back to the client is pretty easy:
```C#
using System.ComponentModel.DataAnnotations;
using EasySockets.Builder;
using EasySockets.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});

app.MapGet("/clients/{roomId}/{clientId}", async (
	string roomId,
	string clientId,
	[FromServices] IEasySocketService easySocketService,
	[Required] [FromQuery] string message) =>
{
	await easySocketService.SendToClientAsync(roomId, clientId, message);
	return $"Send '{message}' to {clientId} in {roomId}";
});

app.Run();
```

This will set the http `/clients/{roomId}/{clientId}` endpoint available. To test the code, do the following:
1. Make a websocket request to `/chat?slug=room0`.
2. When connected, you'll receive a message that could look like the following:
```
Welcome 1783389072. You are currently in room 'room0'
```
3. Copy the client identifier, in this case `1783389072`.
4. Make a http request to `/clients/room0/1783389072?message=hello%20world`.
5. The client connected to `/chat?slug=room0` will receive the `hello world` message.

We can make a check whether the client is actually connected to the server:
```C#
using System.ComponentModel.DataAnnotations;
using EasySockets.Builder;
using EasySockets.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});

app.MapGet("/clients/{roomId}/{clientId}", async (
	string roomId,
	string clientId,
	[FromServices] IEasySocketService easySocketService,
	[Required] [FromQuery] string message) =>
{
	if (!easySocketService.Any(roomId, clientId))
	{
		return $"Client {clientId} not found in room {roomId}";
	}
	await easySocketService.SendToClientAsync(roomId, clientId, message);
	return $"Send '{message}' to {clientId} in {roomId}";
});

app.Run();
```
The `IEasySocketService.Any(string, string)` method will check whether a room with its identifier being equal to the `roomId` exists, whether a client in that room exists where its identifier equals the `clientId` and if that client is connected. If one of these requirements fail (no room present, no client present or the client isn't connected), the result will be that no client has been found.
#### Sending messages to all clients in a room
You can also send messages to all clients in a room. To do this, you need to know the room's identifier. Let's make another endpoint corresponding to this function:

```C#
using System.ComponentModel.DataAnnotations;
using EasySockets.Builder;
using EasySockets.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});

app.MapGet("/clients/{roomId}", async (
	string roomId,
	[FromServices] IEasySocketService easySocketService,
	[Required][FromQuery] string message) =>
{
	if (!easySocketService.Any(roomId))
	{
		return $"Room {roomId} not found";
	}
	await easySocketService.SendToRoomAsync(roomId, message);
	return $"Sent '{message}' to all clients in {roomId}";
});

await app.RunAsync();
```
This will set the http `clients/{roomId}?message=` endpoint open. To test the code, do the following:
1. Make 2 websocket requests to `/chat?slug=room0`.
2. Make an http request to `clients/room0?message=hello%20world`
3. Both clients that are connected to `room0` will receive `hello world`.
#### Disconnecting clients from the server
If needed, you can disconnect clients from these kinds of http endpoints. Be very cautious when using these functions, because the affected clients will instantly disconnect from the server. If these functions are available to everyone, everyone will be able to disconnect clients from the server. So unless the following is exactly what you would want, **Don't apply the following examples in your application.** These are mere examples to showcase the feature.
```C#
using EasySockets.Builder;
using EasySockets.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});


app.MapGet("/clients/disconnect/{roomId}/{clientId}", async (
	string roomId,
	string clientId,
	[FromServices] IEasySocketService service) =>
{
	await service.ForceLeaveAsync(roomId, clientId);
	return $"{clientId} in room '{roomId}' disconnected from the server.";
});

await app.RunAsync();
```
This sets open the `/clients/disconnect{roomId}/{clientId}` http endpoint open. If the service finds a room with the identifier being equal to `roomId` containing a client that has the same identifier as `clientId` and is connected, the service will force a disconnect event to that client.

To test the code, do the following:
1. Make a websocket request to `/chat?slug=room0`.
2. When connected, you'll receive a message that could look like the following:
```
Welcome 1062523944. You are currently in room 'room0'
```
3. Copy the client identifier, in this case `1062523944`.
4. Make a http request to `/clients/disconnect/room0/1062523944`.
5. The client connected to `/chat?slug=room0` will be disconnected from the server.

If needed, you can disconnect all clients in a room from the server:
```C#
using Authentication.Authenticators;
using Authentication.Websockets;
using EasySockets.Builder;
using EasySockets.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEasySocketServices();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseEasySockets()
	.AddEasySocket<ChatSocket>("/chat", options =>
	{
		options.AddAuthenticator<ChatAuthenticator>();
	});


app.MapGet("/clients/disconnect/{roomId}", async (
	string roomId,
	string clientId,
	[FromServices] IEasySocketService service) =>
{
	await service.ForceLeaveAsync(roomId);
	return $"Disconnected all clients in room '{roomId}' from the server.";
});

await app.RunAsync();
```
This works the same as explained in **sending messages to all clients** and **Disconnecting clients**, so we won't go over the full explanation of this functionality.

There are some other methods available in the `IEasySocketService`, some of them are explained here:
1. `IEasySocketService.Any()` checks if any websocket is connected with the server.
2. `IEasySocketService.Any(string)` checks if a room exists with the specified room identifier containing at least 1 connected client.
4. `IEasySocketService.Count()` counts the total number of websocket connections that are connected to the server.
5. `IEasySocketService.Count(string)` counts the total number of websocket connections in a specified room.
6. `IEasySocketService.GetGroupings()` returns an `IEnumerable<IGrouping<string, IEasySocket>>`. This returns all rooms and their websockets. If any custom logic is needed when listing all connections and their states, this can be used.

### Event driven development
Now this is all pretty cool, but the logic in the `ChatSocket` isn't very expandable. If we ever need to implement a lot of custom logic based on the input of a client, we would have to create some sort of fancy switch case, which isn't really optimal nor what we want. This is where event driven development comes in.

In this section, we'll discuss:
1. How to use events.
2. How to set up custom event registration.
3. How to apply this in the `IEasySocketService`.
#### Events
The `EasySocket` class has 3 events available:
1. The `OnConnect` method is invoked when the client successfully connects to the server.
2. The `OnMessage` method is invoked whenever the server receives a message from the client. 
3. The `OnDisconnect` method is invoked when the client disconnects from the server or when the server forces the client to disconnect.

What if we as a developer want more? What if we only want to be notified when the client says `Foo` or `Bar`? Or what if we want to be notified differently based on what the client has to say? This is where events come in.

Events are part of the full message send from the client to the server. An event can contain basically contain anything, so let's pick an example:
```JSON
{
  "event": "Foo",
  "message": "Bar"
}
```
This whole json is sent from the client to the server. The message that needs to be processed is `Bar`, but the event is `Foo`. And this is the part that is so powerful, because we can invoke different pieces of code based on the input of any given client.

#### Event types
Event types are the way events are registered and extracted from the complete message. In the case of `EventSocket`, this is the simple JSON message as shown earlier. When referring to `Event Types`, we are referring to the **structure** of a message. When referring to an `event`, we are referring to the **identifier** or **name** of the event type instance.

Let's say in our application, if a client is typing, we want other clients in his room to know that he is typing. Currently this is not possible. If we would implement code that would send a message to the server whenever a client hits the keyboard, all other clients would receive the full message as a new message. The server doesn't know the difference between a typing event and a message event. It only knows about broadcasting a received message from the client. So lets change that!

We can achieve this by changing our `ChatSocket` subtype from `EasySocket` to `EventSocket`:
```C#
using EasySockets.Events;

public class ChatSocket : EventSocket
{
    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, $"Welcome {ClientId}. You are currently in room '{RoomId}'");
    }
}
```
The `EventSocket` class contains some logic that registers events and invokes code based on one. Since the `EventSocket` processes each message, we can no longer override the `OnMessage` method. If you want, you can override the `OnFailedEventBinding(string)` method. This method is invoked when no registered event is found or when registering of the event fails. In our case this would be when the client sends invalid JSON.
 
In our current code, nothing would happen. This is because we don't have any events registered. The way we do this is with a specific method signature:
```C#
using EasySockets.Enums;
using EasySockets.Events;

namespace EasySocketEvents.Sockets;

public class ChatSocket : EventSocket
{
    public Task OnTyping()
    {
        return Broadcast(new EasySocketEvent("Typing", ClientId + " is typing..."));
    }
        
    public Task OnMessage(EasySocketEvent @event)
    {
        return Broadcast(@event);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, new EasySocketEvent("Connected", $"Welcome {ClientId}. You are currently in room '{RoomId}'"));
    }
}
```
Any method with the `On` suffix will be registered except the EasySocket methods like `OnConnect` and `OnDisconnect`.
Whenever a message with the event `Typing` is received, the `OnTyping` method will be invoked. Whenever the event `Message` is received, the `OnMessage` method is invoked with the current event as the parameter.

*Note: If a `Task` is returned, it will be awaited but if any other object is returned, it is disregarded. Returned objects will not be sent over a websocket connection since the system does not know whether you want it to be sent to the client or to the room and in what format.*

*Another note: The methods parameters should always be of the event type (by default `EasySocketEvent`) or a string. Other parameters cannot be resolved and will always result in `null`. If the event type parameter is present, the instance of the current event type will be invoked. If the string parameter is present, the whole received plaintext message is invoked. If you want DI to be available, it is recommended to inject the `IServiceScopeFactory` interface in the constructor and use it to resolve the required services in the method.*

To test this, do the following:
1. Make 2 websocket requests to `/chat`.
2. Send the following JSON over the websocket:
```JSON
{
	"Event": "Typing",
	"Message": ""
}
```
3. Other clients will receive the following JSON (with the `{ClientId}` section replaced with a random guid):
```JSON
{
	"Event": "Typing",
	"Message": "{ClientId} is typing..."
}
```
5. Send the following JSON over the websocket:
```JSON
{
	"Event": "Message",
	"Message": "Hello World!"
}
```
5. Other clients will receive that exact event.

Great! Now all clients can implement logic to change different elements of the UI to accommodate for different events.

If you want to name your methods differently by leaving out the `On` suffix, you can do so by using the `InvokeOnAttribute` attribute:

```CSharp
using EasySockets.Attributes;
using EasySockets.Enums;
using EasySockets.Events;

namespace EasySocketEvents.Sockets;

public class ChatSocket : EventSocket
{
    [InvokeOn("Typing")]
    public Task SomeoneTyped()
    {
        return Broadcast(new EasySocketEvent("Typing", ClientId + " is typing..."));
    }

    [InvokeOn("Message")]
    public Task MessageReceived(EasySocketEvent @event)
    {
        return Broadcast(@event);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, new EasySocketEvent("Connected", $"Welcome {ClientId}. You are currently in room '{RoomId}'"));
    }
}
```
The code above works exactly the same as when the method names had the `On` suffix.

*Note: You can apply as many `InvokeOn` attributes as you want on a single method. Each time one of the events is received, the method is invoked. Just be careful not to have duplicate events names present on a single EasySocket class.*

### Event type registration
Now this is cool, but not really customizable. Currently, our event structure looks like the following:
```JSON
{
	"Event": "event",
	"Message": "message"
}
```
If any field is missing or more fields are added to this JSON, the server would reject the message and not invoke anything. To change this we can register our own custom event type. This means we can change the whole structure of a message with little changes to our code base. This is extremely powerful since you can optimize your websocket connection to your needs. For example, if you need less overhead when receiving messages you could restructure your messages and reduce overhead to a single character:
```
event:message
```
To showcase the feature, we are going to restructure the event type registration using XML. Know that you can completely customize this process to your needs.

First off, we need a class that represents the event. To do this, create a class that implements the `IEasySocketEvent` interface:
```C#
using EasySockets.Events;

public class XmlEvent : IEasySocketEvent
{
    public string Event { get; set; } = null!;
    public string Message { get; set; } = null!;
}
```
The only property that needs to be implemented is the `Event` getter. This is used to find corresponding methods that can then be invoked.

Now we need a class that encapsulates the logic to parse events. Create a class that extends the `EventSocket<XmlEvent>` class:
```C#
using EasySockets.Events;

public class XmlEventSocket : EventSocket<XmlEvent>
{
    public override XmlEvent? ExtractEvent(string message)
    {
        throw new NotImplementedException();
    }

    public override string? BindEvent(string @event, string message)
    {
        throw new NotImplementedException();
    }
}
```
A few methods need to be implemented, so let's go over them:
* The `ExtractEvent` method needs to parse a fully received message into an object given into the generic type of the `EventSocket<TEvent>` class. If one of the event methods accepts this object's type as a parameter, the object returned will be injected into the method. If extracting fails, you should return `null`. If at any point during the websocket connection any exception is thrown, the websocket connection will close and result in a `500 - Internal Server Error` status code.
* The `BindEvent` method combines an event and a message together into a single string that can be sent over the websocket connection.

In our case, we want to simply deserialize an XML string into an object in the `ExtractEvent` method, and serialize the object into a XML string in the `BindEvent` method:
```C#
using System.Xml.Serialization;
using EasySockets.Events;

public class XmlEventSocket : EventSocket<XmlEvent>
{
    private static readonly XmlSerializer Serializer = new(typeof(XmlEvent));

    public override XmlEvent? ExtractEvent(string message)
    {
        try
        {
            using var reader = new StringReader(message);
            return (XmlEvent?)Serializer.Deserialize(reader);
        }
        catch
        {
            return null;
        }
    }

    public override string? BindEvent(string @event, string message)
    {
        var xmlEvent = new XmlEvent
        {
            Event = @event,
            Message = message
        };

        using var writer = new StringWriter();
        
        Serializer.Serialize(writer, xmlEvent);
        
        return writer.ToString();
    }
}
```
Great, now we have fully configured our custom event binder! Let's change our `ChatSocket` subtype to that of the `XmlEventSocket` type:
```C#
using EasySockets.Enums;

public class ChatSocket : XmlEventSocket
{
    public Task OnTyping()
    {
        return Broadcast("Typing", ClientId + " is typing...");
    }

    public Task OnMessage(XmlEvent @event)
    {
        return Broadcast(@event.Event, @event.Message);
    }

    public override Task OnConnect()
    {
        return Broadcast(BroadCastFilter.EqualRoomId, "Connected", $"Welcome {ClientId}. You are currently in room '{RoomId}'");
    }
}
```
Notice a small difference; we aren't passing the `XmlEvent` instance down to the `Broadcast` method. Since EasySocket does not know what represents the message part in an event instance, this method isn't implemented in the `EasySocket<TEvent>` class. If you wish you can implement this for yourself, but for the sake of simplicity in the tutorial we won't go over this process.

Now let's test our newly created custom event binding websocket connection!
1. Make 2 websocket requests to `/chat?roomId=room0`
2. Send the following XML:
```XML
<XmlEvent>
    <Event>Typing</Event>
</XmlEvent>
```
3. Other clients will receive the following XML back:
```XML
<?xml version="1.0" encoding="utf-16"?>
<XmlEvent xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Event>Typing</Event>
  <Message>{ClientId} is typing...</Message>
</XmlEvent>
```

And that's all their is to `EasySockets`. Feel free to give this project a star if you liked it. If you have any suggestions for new features or code changes let me know!