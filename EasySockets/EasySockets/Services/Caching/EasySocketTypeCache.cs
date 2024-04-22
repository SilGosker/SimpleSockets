using System.Collections.ObjectModel;
using EasySockets.Builder;

namespace EasySockets.Services.Caching;

internal class EasySocketTypeCache
{
    internal Type EasySocketType { get; init; }
    internal IReadOnlyList<Type> AuthenticatorTypes { get; init; }
    internal EasySocketOptions Options { get; init; }
    internal EasySocketTypeCache(Type easySocketType, EasySocketOptions options)
    {
        EasySocketType = easySocketType;
        Options = options;
        AuthenticatorTypes = new ReadOnlyCollection<Type>(Options.Authenticators);
    }
}