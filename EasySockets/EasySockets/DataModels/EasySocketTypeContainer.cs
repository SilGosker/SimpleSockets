using System.Reflection;
using EasySockets.Builder;

namespace EasySockets.DataModels;

internal sealed class EasySocketTypeContainer
{
    internal ParameterInfo[] ConstructorDependencies { get; set; }
    internal Type EasySocketType { get; set; }
    internal IReadOnlyList<Type> AuthenticatorTypes { get; set; }
    internal EasySocketOptions Options;
    
    private EasySocketTypeContainer(Type simpleSocketType, EasySocketOptions? options)
    {
        EasySocketType = simpleSocketType;
        var constructorParameters = simpleSocketType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();
        if (constructorParameters == null) throw new InvalidOperationException($"No constructors for type {simpleSocketType.FullName} found. Check if the class and constructor is public.");
        ConstructorDependencies = constructorParameters.GetParameters();
        Options = options ?? new EasySocketOptions();
        AuthenticatorTypes = Options.Authenticators;
    }

    internal static EasySocketTypeContainer Create(Type simpleSocketType, EasySocketOptions? options)
    {
        return new (simpleSocketType, options);
    }
}