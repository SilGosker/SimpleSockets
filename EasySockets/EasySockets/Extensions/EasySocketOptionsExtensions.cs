using EasySockets.Builder;

namespace EasySockets.Extensions;

public static class EasySocketOptionsExtensions
{
    public static ReadonlyEasySocketOptions AsReadonly(this EasySocketOptions options)
    {
        return new(options);
    }
}