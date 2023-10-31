using System.Reflection;
using EasySockets.Builder;

namespace EasySockets.DataModels;

internal sealed class EasySocketTypeContainer
{
    internal Type EasySocketType { get; set; }
    internal IReadOnlyList<Type> AuthenticatorTypes { get; set; }
    internal EasySocketOptions Options { get; set; }
    
    internal EasySocketTypeContainer(Type simpleSocketType, EasySocketOptions? options)
    {
        EasySocketType = simpleSocketType;
        Options = options ?? new EasySocketOptions();
        AuthenticatorTypes = Options.Authenticators;
    }
}