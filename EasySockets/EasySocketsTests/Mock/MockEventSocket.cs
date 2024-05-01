using EasySockets.Events;

namespace EasySockets.Mock;

public class MockEventSocket : EventSocket<MockEvent>
{
    public virtual Task OnEventAsync(string _)
    {
        return Task.CompletedTask;
    }

    public virtual void OnEvent(string _)
    {

    }

    public override MockEvent? ExtractEvent(string message)
    {
        throw new NotImplementedException();
    }

    public override string? BindEvent(string @event, string message)
    {
        throw new NotImplementedException();
    }
}