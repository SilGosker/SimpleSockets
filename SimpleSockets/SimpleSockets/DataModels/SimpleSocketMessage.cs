using SimpleSockets.Enums;

namespace SimpleSockets.DataModels;

public class SimpleSocketMessage
{
    internal SimpleSocketMessage(BroadCastLevel level)
    {
        BroadCastLevel = level;
    }

    internal  SimpleSocketMessage(string @event, string message) : this(BroadCastLevel.Members)
    {
        Event = @event;
        Message = message;
    }
    
    internal  SimpleSocketMessage(string @event, string message, BroadCastLevel broadCastLevel) : this(broadCastLevel)
    {
        Event = @event;
        Message = message;
    }
    public string Message { get; set; }
    public string Event { get; set; }
    public BroadCastLevel BroadCastLevel { get; set; }
    public string Sender { get; set; }
}