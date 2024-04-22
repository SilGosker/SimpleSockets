using EasySockets.Builder;
using EasySockets.Mock;

namespace EasySockets.Services.Caching;

public class EasySocketTypeCacheTests
{
    [Fact]
    public void Constructor_ShouldSetEasySocketType()
    {
        var easySocketType = typeof(MockEasySocket);
        var options = new EasySocketOptions();
        var easySocketTypeCache = new EasySocketTypeCache(easySocketType, options);

        Assert.Equal(easySocketType, easySocketTypeCache.EasySocketType);
        Assert.Equal(options, easySocketTypeCache.Options);
    }
}