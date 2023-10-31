using System.Collections.ObjectModel;
using EasySockets.Builder;

namespace EasySockets.DataModels;

internal sealed class EasySocketTypeCache
{
    internal Type EasySocketType { get; set; }
    internal IReadOnlyList<Type> AuthenticatorTypes { get; init; }
    internal EasySocketOptions Options { get; set; }
    
    internal EasySocketTypeCache(Type simpleSocketType, EasySocketOptions? options)
    {
        EasySocketType = simpleSocketType;
        Options = options ?? new EasySocketOptions();
        AuthenticatorTypes = new ReadOnlyCollection<Type>(Options.Authenticators);
    }
}