using System.Runtime.CompilerServices;
using System.Text;
using EasySockets.Services;
using EasySockets.Services.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EasySockets.Helpers;

internal static class ThrowHelper
{
    internal static void ThrowIfNull<T>(T value, [CallerArgumentExpression("value")] string parameterName = null!)
    {
        if (value == null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    internal static void ThrowIfCannotFitEncodingChars(int value, Encoding encoding,
        [CallerArgumentExpression("value")] string parameterName = null!)
    {
        if (value < encoding.GetMaxByteCount(1))
        {
            throw new ArgumentOutOfRangeException(parameterName,
                $"The size of the {parameterName} is too small for {encoding.GetType().Name} Encoding");
        }
    }

    internal static void ThrowIfInvalidRoomId(string? currentRoomId, string fallbackRoomId)
    {
        if (currentRoomId == null && fallbackRoomId == null)
        {
            throw new InvalidOperationException(
                "The authenticationResult.RoomId and the default roomId should not be null after successful authentication");
        }
    }

    internal static void ThrowIfInvalidClientId(string? currentClientId, string fallbackClientId)
    {
        if (currentClientId == null && fallbackClientId == null)
        {
            throw new InvalidOperationException(
                "The authenticationResult.ClientId and the default userId should not be null after successful authentication");
        }
    }

    internal static void ThrowIfServiceCollectionIsInitialized(IServiceCollection serviceCollection)
    {
        if (serviceCollection.Any(x => x.ServiceType == typeof(IEasySocketService)))
        {
            throw new InvalidOperationException(
                               "The IEasySocketService has already been added to the service collection.");
        }
    }

    internal static void ThrowIfUrlIsAddedTwice(IDictionary<PathString, EasySocketTypeCache> values, string url)
    {
        if (values.Any(x => x.Key == url))
        {
            throw new InvalidOperationException($"The url '{url}' has already been added to the service collection.");
        }
    }
}