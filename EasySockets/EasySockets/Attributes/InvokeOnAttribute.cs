using EasySockets.Helpers;

namespace EasySockets.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InvokeOnAttribute : Attribute
{
    public InvokeOnAttribute(string @event)
    {
        ThrowHelper.ThrowIfNull(@event);
        Event = @event;
    }
    internal string Event { get; }
}