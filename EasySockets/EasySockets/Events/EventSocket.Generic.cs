using EasySockets.DataModels;
using EasySockets.Enums;

namespace EasySockets.Events;

public abstract class EventSocket<TEvent> : EasySocket, IEventSocket where TEvent : IEasySocketEvent
{

	/// <summary>
	///     Sends a message to the client websocket.
	/// </summary>
	/// <param name="message">The message to be sent.</param>
	/// <param name="event">The event id/name</param>
	/// <returns>A task representing the asynchronous operation of sending the message to the client.</returns>
	public Task SendToClient(string message, string @event)
    {
	    return SendToClientAsync(BindEvent(message, @event) ?? "");
    }

    /// <inheritdoc cref="SendToClient(string,string)" />
    public async Task SendToClient(string message, TEvent @event)
    {
        await SendToClient(message, @event.GetEvent());
    }

    /// <summary>
    ///     Adds an event with an function to be executed when the event is triggered.
    /// </summary>
    /// <param name="event">The event object with an extracted event property in it</param>
    /// <param name="action">
    ///     The action that will be executed once the event is triggered containing the message as a string.
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    protected void On(TEvent @event, Func<string, Task> action)
    {
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract : Never trust the users input
        if (@event is null || @event.GetEvent() is null) throw new ArgumentNullException(nameof(@event));
        // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

        if (action is null) throw new ArgumentNullException(nameof(action));
        var eventName = @event.GetEvent();
        if (EasySocketEventHolder.Events.ContainsKey(new EasySocketEventComparer(GetType(), eventName)))
            return;

        EasySocketEventHolder.Events.Add(new EasySocketEventComparer(GetType(), eventName), action);
    }

    /// <summary>
    ///     Sends a message with an event id/name to the members matching the all members specified by the
    ///     <paramref name="filter" />
    /// </summary>
    /// <param name="filter">The broadcast level the message will reach</param>
    /// <param name="event">The event id/name</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The task representing the parallel asynchronous sending</returns>
    public Task Broadcast(BroadCastFilter filter, string @event, string message)
    {
        return Broadcast(filter, BindEvent(@event, message) ?? "");
    }

	/// <summary>
	///     Sends a message to the members matching the <see cref="EasySocket.RoomId" />.
	/// </summary>
	/// <inheritdoc cref="Broadcast(BroadCastFilter, string, string)" />
	public Task Broadcast(string @event, string message)
    {
        return Broadcast(BroadCastFilter.EqualRoomId, BindEvent(@event, message) ?? "");
    }

    public sealed override async Task OnMessage(string message)
    {
        var @event = ExtractEvent(message);
        if (@event is null)
        {
            await OnFailedEventBinding(message);
            return;
        }

        var eventComparer = new EasySocketEventComparer(GetType(), @event.GetEvent());
        KeyValuePair<EasySocketEventComparer, Func<string, Task>>? action =
            EasySocketEventHolder.Events.FirstOrDefault(e => e.Key == eventComparer);

        if (action is null)
        {
            await OnFailedEventBinding(message);
            return;
        }

        var extractedMessage = ExtractMessage(@event, message);
        if (extractedMessage is null)
        {
            await OnFailedEventBinding(message);
            return;
        }

        await action.Value.Value.Invoke(extractedMessage);
    }

    /// <summary>
    ///     Extracts the event from the message. <br />
    ///     For example, the <paramref name="message" /> could contain JSON like this:
    ///     <code>
    /// {
    ///   "message": "foo",
    ///   "event": "bar"
    /// }
    ///     </code>
    ///     This function should return <c>bar</c> as the result, as that is the extracted event.
    /// </summary>
    /// <param name="message">The full message received from the client</param>
    /// <returns>The event as a string, or <c>null</c> if the operation failed.</returns>
    public abstract TEvent? ExtractEvent(string message);

    /// <summary>
    ///     Extracts the message from the given parameters. <br />
    ///     For example, the <paramref name="message" /> could contain JSON like this:
    ///     <code>
    /// {
    ///   "message": "foo",
    ///   "event": "bar"
    /// }
    ///     </code>
    ///     This function should return <c>foo</c> as the result, as that is the extracted message.
    /// </summary>
    /// <param name="event">The event that was registered from <see cref="ExtractEvent" /></param>
    /// <param name="message">The full message received from the client</param>
    /// <returns>The text message, or null if the operation failed.</returns>
    public abstract string? ExtractMessage(TEvent @event, string message);

    /// <summary>
    ///     Creates a message based on the given parameters. <br />
    ///     In the example used in <br /> <see cref="ExtractEvent" />  and <br /><see cref="ExtractMessage" />,<br /> the code
    ///     should return the following JSON:
    ///     <code>
    /// {
    ///   "message": "foo",
    ///   "event": "bar"
    /// }
    ///     </code>
    /// </summary>
    /// <param name="event">The event id/name</param>
    /// <param name="message">The message to be sent</param>
    /// <returns>The full message to be sent over the connection, or null if the operation failed</returns>
    public abstract string? BindEvent(string @event, string message);

    /// <summary>
    ///     When the event extraction fails or is not registered, this function will be called.
    /// </summary>
    /// <param name="message">The full message received from the backing websocket</param>
    /// <returns>A task representing the execution.</returns>
    public virtual Task OnFailedEventBinding(string message)
    {
        return Task.CompletedTask;
    }
}