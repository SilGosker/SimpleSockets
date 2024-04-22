namespace EasySockets.Events;

/// <summary>
///     The interface for a EasySocket class with custom event implementation.
/// </summary>
public interface IEventSocket : IEasySocket, IInternalEventSocket
{
    /// <summary>
    ///     Sends a message with an event id/name to the client.
    /// </summary>
    /// <param name="message">The content of the message</param>
    /// <param name="event">The event id/name to be sent</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task SendToClientAsync(string message, string @event);
}