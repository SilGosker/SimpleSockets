using EasySockets.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EasySockets.Builder;

public class AppBuilderExtensionsTests
{
    [Fact]
    public void UseEasySockets_WhenEasySocketServicesAreAdded_ShouldReturnEasySocketBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEasySocketServices();

        var serviceProvider = services.BuildServiceProvider();
        var app = new Mock<IApplicationBuilder>();
        app.Setup(x => x.ApplicationServices).Returns(serviceProvider);

        // Act
        var result = app.Object.UseEasySockets();

        // Assert
        Assert.NotNull(result);
    }


    [Fact]
    public void UseEasySockets_WhenEasySocketServicesAreNotAdded_ShouldThrowException()
    {
        var app = new Mock<IApplicationBuilder>();
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        app.Setup(x => x.ApplicationServices).Returns(serviceProvider);

        Assert.Throws<InvalidOperationException>(app.Object.UseEasySockets);
    }
}