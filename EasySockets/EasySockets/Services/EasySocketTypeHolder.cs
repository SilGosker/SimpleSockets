using EasySockets.DataModels;

namespace EasySockets.Services;

public class EasySocketTypeHolder
{
    private readonly IDictionary<string, EasySocketTypeCache> _easySocketTypes = new Dictionary<string, EasySocketTypeCache>();

    internal void AddType(string url, EasySocketTypeCache simpleSocketType)
    {
        if (_easySocketTypes.ContainsKey(url)) throw new InvalidOperationException($"Url '{url}' Cannot be added twice");
        _easySocketTypes.Add(url, simpleSocketType);
    }

    public bool TryGetValue(string url, out EasySocketTypeCache easySocketTypeCache)
    {
        return _easySocketTypes.TryGetValue(url, out easySocketTypeCache!);
    }
}