namespace EasySockets.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InvokeOnAttribute : Attribute
{
    public InvokeOnAttribute(string @event)
    {
        Event = @event ?? throw new ArgumentNullException(nameof(@event));
    }
    internal string Event { get; }
}