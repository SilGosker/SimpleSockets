using EasySockets.Events;

namespace EasySockets.Mock;

public class MockEvent : IEasySocketEvent
{
    public string Event { get; set; } = string.Empty;
}