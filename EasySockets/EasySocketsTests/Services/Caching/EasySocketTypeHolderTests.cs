using EasySockets.Builder;
using EasySockets.Mock;

namespace EasySockets.Services.Caching;

public class EasySocketTypeHolderTests
{
    [Fact]
    public void AddType_WhenUrlAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var url = "/url";
        var easySocketTypeHolder = new EasySocketTypeHolder();
        var easySocketTypeCache = new EasySocketTypeCache(typeof(MockEasySocket), new EasySocketOptions());
        easySocketTypeHolder.AddType(url, easySocketTypeCache);

        Assert.Throws<InvalidOperationException>(() => easySocketTypeHolder.AddType(url, easySocketTypeCache));
    }

    [Fact]
    public void TryGetValue_WhenUrlDoesNotExist_ShouldReturnFalse()
    {
        var url = "/url";
        var easySocketTypeHolder = new EasySocketTypeHolder();

        Assert.False(easySocketTypeHolder.TryGetValue(url, out _));
    }

    [Fact]
    public void TryGetValue_WhenUrlExists_ShouldReturnTrue()
    {
        var url = "/url";
        var easySocketTypeHolder = new EasySocketTypeHolder();
        var easySocketTypeCache = new EasySocketTypeCache(typeof(MockEasySocket), new EasySocketOptions());
        easySocketTypeHolder.AddType(url, easySocketTypeCache);

        Assert.True(easySocketTypeHolder.TryGetValue(url, out _));
    }

    [Fact]
    public void TryGetValue_WhenUrlExists_ShouldReturnEasySocketTypeCache()
    {
        var url = "/url";
        var easySocketTypeHolder = new EasySocketTypeHolder();
        var easySocketTypeCache = new EasySocketTypeCache(typeof(MockEasySocket), new EasySocketOptions());
        easySocketTypeHolder.AddType(url, easySocketTypeCache);

        Assert.True(easySocketTypeHolder.TryGetValue(url, out var result));
        Assert.Equal(easySocketTypeCache, result);
    }
}