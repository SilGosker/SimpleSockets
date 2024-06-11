using EasySockets.Builder;

namespace EasySockets.Extensions;

public static class EasySocketOptionsExtensions
{
    public static ReadonlyEasySocketOptions AsReadonly(this EasySocketOptions options)
    {
        return new()
        {
            ClosingStatusDescription = options.ClosingStatusDescription,
            Encoding = options.Encoding,
            ReceiveBufferSize = options.ReceiveBufferSize,
            SendBufferSize = options.SendBufferSize,
        };
    }
}