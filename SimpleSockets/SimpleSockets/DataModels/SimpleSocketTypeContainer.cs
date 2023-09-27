using System.Reflection;
using SimpleSockets.Builder;

namespace SimpleSockets.DataModels;

internal sealed class SimpleSocketTypeContainer
{
    internal ParameterInfo[] ConstructorDependencies { get; set; }
    internal Type SimpleSocketType { get; set; }
    internal IReadOnlyList<Type> AuthenticatorTypes { get; set; }
    internal SimpleSocketOptions Options;
    
    private SimpleSocketTypeContainer(Type simpleSocketType, SimpleSocketOptions? options)
    {
        SimpleSocketType = simpleSocketType;
        var constructorParameters = simpleSocketType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();
        if (constructorParameters == null) throw new InvalidOperationException($"No constructors for type {simpleSocketType.FullName} found. Check if the class and constructor is public.");
        ConstructorDependencies = constructorParameters.GetParameters();
        Options = options ?? new SimpleSocketOptions();
        AuthenticatorTypes = Options.Authenticators;
    }

    internal static SimpleSocketTypeContainer Create(Type simpleSocketType, SimpleSocketOptions? options)
    {
        return new (simpleSocketType, options);
    }
}