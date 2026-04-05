using BeautifulClient.UI.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace BeautifulClient.UI;

public sealed class ConsoleHost(IServiceProvider serviceProvider)
{
    public async Task Host<TController>(object? settings = null) where TController : Controller
    {
        // Start the app at the HomePageController
        Type? currentRouteType = typeof(TController);
        object? payload = null;

        while (currentRouteType != null)
        {
            // Resolve the requested controller from Dependency Injection
            IRouter controller = (IRouter) serviceProvider.GetRequiredService(currentRouteType);

            // Execute it, and wait for it to tell us where to go next.
            NavigationResult result = await controller.ExecuteAsync(payload);
            currentRouteType = result.NextRoute;
            payload = result.Payload;
        }
    }
}