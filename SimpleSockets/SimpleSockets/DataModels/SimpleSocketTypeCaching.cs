using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace SimpleSockets.DataModels;

internal class SimpleSocketTypeCaching
{
    internal ParameterInfo[] ConstructorDependencies { get; set; }
    internal MethodInfo OnAuthentication { get; set; }
    internal Type Type { get; set; }
    private SimpleSocketTypeCaching(Type type)
    {
        Type = type;
        ConstructorDependencies = type.GetConstructors()[0].GetParameters();
        OnAuthentication = type.GetMethod(nameof(SimpleSocket.Authenticate), BindingFlags.Static)!;
    }
    internal static SimpleSocketTypeCaching Create<T>() where T : SimpleSocket
    {
        return new SimpleSocketTypeCaching(typeof(T));
    }
}