using System.Reflection;
using Microsoft.AspNetCore.Http;
using SimpleSockets.Interfaces;

namespace SimpleSockets.DataModels;

internal sealed class SimpleSocketTypeCaching
{
    internal ParameterInfo[] ConstructorDependencies { get; set; }
    internal Type SimpleSocketType { get; set; }
    public Type? AuthenticatorType { get; set; }
    private SimpleSocketTypeCaching(Type simpleSocketType, Type? authenticatorType)
    {
        SimpleSocketType = simpleSocketType;
        var constructorParameters = simpleSocketType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();
        if (constructorParameters == null) throw new InvalidOperationException($"No constructors for type {simpleSocketType.FullName} found. Check if the class and constructor is public.");
        ConstructorDependencies = constructorParameters.GetParameters();
        AuthenticatorType = authenticatorType;
    }
    internal static SimpleSocketTypeCaching Create<TSimpleSocket>()
        where TSimpleSocket : ISimpleSocket
    {
        return new SimpleSocketTypeCaching(typeof(TSimpleSocket), null);
    }
    internal static SimpleSocketTypeCaching Create<TSimpleSocket, TAuthenticator>()
        where TSimpleSocket : ISimpleSocket
        where TAuthenticator : ISimpleSocketAsyncAuthenticator
    {
        return new SimpleSocketTypeCaching(typeof(TSimpleSocket), typeof(TAuthenticator));
    }
}