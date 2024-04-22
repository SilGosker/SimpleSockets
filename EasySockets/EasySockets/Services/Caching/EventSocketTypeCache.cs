using System.Reflection;
using EasySockets.Attributes;
using EasySockets.Events;

namespace EasySockets.Services.Caching;

internal sealed class EventSocketTypeCache : EasySocketTypeCache
{
    internal IReadOnlyList<EventSocketEventInfo> EventInfos { get; }

    private bool IsEventMethod(MethodInfo method)
    {
        if (method.DeclaringType == null
            || method.DeclaringType == typeof(EasySocket)
            || method.DeclaringType == typeof(EventSocket)
            || (method.DeclaringType.IsGenericType && method.DeclaringType.GetGenericTypeDefinition() == typeof(EventSocket<>))
            || (method.IsVirtual && method.GetBaseDefinition().DeclaringType == typeof(EasySocket)))
        {
            return false;
        }

        return method.GetCustomAttributes<InvokeOnAttribute>().Any()
            || (method.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase) && method.Name.Length > 2);
    }

    internal EventSocketTypeCache(EasySocketTypeCache cache) : base(cache.EasySocketType, cache.Options)
    {
        var methods = EasySocketType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(IsEventMethod)
            .ToArray();

        var eventInfos = new List<EventSocketEventInfo>();
        foreach (var method in methods)
        {
            var attributes = method.GetCustomAttributes<InvokeOnAttribute>().ToArray();

            if (attributes.Length == 0)
            {
                eventInfos.Add(new EventSocketEventInfo(method, new[] { method.Name[2..] }));
                continue;
            }

            var eventNames = new string[attributes.Length];

            for (var j = 0; j < attributes.Length; j++)
            {
                eventNames[j] = attributes[j].Event;
            }

            eventInfos.Add(new EventSocketEventInfo(method, eventNames));
        }

        EventInfos = eventInfos.AsReadOnly();
    }
}