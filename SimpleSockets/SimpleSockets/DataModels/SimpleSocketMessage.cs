using SimpleSockets.Enums;

namespace SimpleSockets.DataModels;

internal class SimpleSocketMessage
{
    internal SimpleSocketMessage(BroadCastLevel level, string? @event, string message)
    {
        BroadCastLevel = level;
        Event = @event;
        Message = message;
    }

    public string? Message { get; set; }
    public string? Event { get; set; }
    public BroadCastLevel BroadCastLevel { get; set; }
    public string? Sender { get; set; }
}