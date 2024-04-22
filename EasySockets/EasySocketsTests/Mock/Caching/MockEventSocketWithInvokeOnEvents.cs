#pragma warning disable CA1822 // Mark members as static
using EasySockets.Attributes;
using EasySockets.Events;

namespace EasySockets.Mock.Caching;

public class MockEventSocketWithInvokeOnEvents : EventSocket
{
    [InvokeOn("Event")]
    // ReSharper disable once UnusedMember.Global
    public void Event()
    {

    }
}