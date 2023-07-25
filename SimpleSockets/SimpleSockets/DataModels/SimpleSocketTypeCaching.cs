using System.Reflection;
using Microsoft.AspNetCore.Http;
using SimpleSockets.Interfaces;
using SimpleSockets.Options;

namespace SimpleSockets.DataModels;

internal sealed class SimpleSocketTypeCaching
{
    internal ParameterInfo[] ConstructorDependencies { get; set; }
    internal Type SimpleSocketType { get; set; }
    internal Type? AuthenticatorType { get; set; }
    internal SimpleSocketOptions Options;
    
    private SimpleSocketTypeCaching(Type simpleSocketType, SimpleSocketOptions? options)
    {
        SimpleSocketType = simpleSocketType;
        var constructorParameters = simpleSocketType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();
        if (constructorParameters == null) throw new InvalidOperationException($"No constructors for type {simpleSocketType.FullName} found. Check if the class and constructor is public.");
        ConstructorDependencies = constructorParameters.GetParameters();
        Options = options ?? new SimpleSocketOptions();
        AuthenticatorType = Options.AuthenticatorType;
    }

    internal static SimpleSocketTypeCaching Create(Type simpleSocketType, SimpleSocketOptions? options)
    {
        return new (simpleSocketType, options);
    }
}