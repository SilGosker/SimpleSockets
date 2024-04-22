using System.Reflection;
using EasySockets.Events;

namespace EasySockets.Services.Caching;

internal class EventSocketEventInfo
{
    private readonly MethodInfo _method;
    private readonly HashSet<string> _eventNames;
    private readonly ParameterInfo[] _parameters;
    internal EventSocketEventInfo(MethodInfo method, string[] eventNames)
    {
        _method = method;
        _parameters = method.GetParameters();
        _eventNames = new HashSet<string>(eventNames);
    }

    internal bool Contains(string eventName)
    {
        return _eventNames.Contains(eventName);
    }

    internal Task InvokeAsync<TEvent>(EventSocket<TEvent> instance, TEvent @event, string message) where TEvent : IEasySocketEvent
    {
        if (_parameters.Length == 0)
        {
            var result = _method.Invoke(instance, Array.Empty<object>());
            if (result is Task t)
            {
                return t;
            }
            return Task.CompletedTask;
        }

        var parameters = new object[_parameters.Length];

        for (var i = 0; i < _parameters.Length; i++)
        {
            var parameter = _parameters[i];

            if (parameter.ParameterType == typeof(TEvent))
            {
                parameters[i] = @event;
                continue;
            }
            
            if (parameter.ParameterType == typeof(string))
            {
                parameters[i] = message;
            }
        }

        var result2 = _method.Invoke(instance, parameters);
        if (result2 is Task t2)
        {
            return t2;
        }
        return Task.CompletedTask;
    }
}