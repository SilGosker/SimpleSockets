using EasySockets.Events;
using EasySockets.Helpers;
using Microsoft.AspNetCore.Http;

namespace EasySockets.Services.Caching;

internal class EasySocketTypeHolder
{
    private readonly IDictionary<PathString, EasySocketTypeCache> _easySocketTypes = new Dictionary<PathString, EasySocketTypeCache>();

    internal void AddType(string url, EasySocketTypeCache easySocketType)
    {
        if (_easySocketTypes.ContainsKey(url)) throw new InvalidOperationException($"Url '{url}' Cannot be added twice");

        if (TypeHelper.IsSubclassOfRawGeneric(easySocketType.EasySocketType, typeof(EventSocket<>)))
        {
            _easySocketTypes.Add(url, new EventSocketTypeCache(easySocketType));
            return;
        }

        _easySocketTypes.Add(url, easySocketType);
    }

    internal bool TryGetValue(string url, out EasySocketTypeCache easySocketTypeCache)
    {
        return _easySocketTypes.TryGetValue(url, out easySocketTypeCache!);
    }
}