namespace EasySockets.Events;

/// <summary>
///     The interface for a EasySocket class with custom event implementation.
/// </summary>
public interface IEventSocket : IEasySocket, IInternalEventSocket
{
   /// <inheritdoc cref="SendToClientAsync(string,string, CancellationToken)"/>
   public Task SendToClientAsync(string @event, string message);

    /// <summary>
    ///     Sends a message with an event to the client.
    /// </summary>
    /// <param name="message">The content of the message.</param>
    /// <param name="event">The event identifier or name.</param>
    /// <param name="cancellationToken">The <seealso cref="CancellationToken" /> to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SendToClientAsync(string @event, string message, CancellationToken cancellationToken);
}