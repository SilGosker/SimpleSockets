using EasySockets.Builder;
using EasySockets.Mock.Caching;

namespace EasySockets.Services.Caching;

public class EventSocketTypeCacheTests
{
    [Fact]
    public void Constructor_RegistersMethodsStartingWithOn()
    {
        var easySocketTypeCache = new EasySocketTypeCache(typeof(MockEventSocketWithOnEvents), new EasySocketOptions());
        var cache = new EventSocketTypeCache(easySocketTypeCache);

        Assert.Single(cache.EventInfos);
    }

    [Fact]
    public void Constructor_ExcludesEasySocketMethods()
    {
        var easySocketTypeCache = new EasySocketTypeCache(typeof(MockEventSocketWithoutEvents), new EasySocketOptions());
        var cache = new EventSocketTypeCache(easySocketTypeCache);

        Assert.Empty(cache.EventInfos);
    }

    [Fact]
    public void Constructor_RegistersMethodsWithInvokeOnAttribute()
    {
        var easySocketTypeCache = new EasySocketTypeCache(typeof(MockEventSocketWithInvokeOnEvents), new EasySocketOptions());
        var cache = new EventSocketTypeCache(easySocketTypeCache);
        
        Assert.Single(cache.EventInfos);
    }
}