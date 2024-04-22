using EasySockets.Services.Caching;

namespace EasySockets.Events;

public interface IInternalEventSocket
{
    internal IReadOnlyList<EventSocketEventInfo> Events { set; }
}