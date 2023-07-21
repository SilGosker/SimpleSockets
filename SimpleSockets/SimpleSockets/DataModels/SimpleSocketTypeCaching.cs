using System.Reflection;
using Microsoft.AspNetCore.Http;
using SimpleSockets.Interfaces;

namespace SimpleSockets.DataModels;

internal class SimpleSocketTypeCaching
{
    internal ParameterInfo[] ConstructorDependencies { get; set; }
    internal Type SimpleSocketType { get; set; }
    public Type? AuthenticatorType { get; set; }
    private SimpleSocketTypeCaching(Type simpleSocketType, Type? authenticatorType)
    {
        SimpleSocketType = simpleSocketType;
        ConstructorDependencies = simpleSocketType.GetConstructors()[0].GetParameters();
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