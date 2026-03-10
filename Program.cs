using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeautifulClient;

internal class Program
{
    private static void Main(string[] args)
    {
        // 1. Create the builder (similar to WebApplication.CreateBuilder)
        var builder = Host.CreateApplicationBuilder(args);

        // 2. Add your dependencies to the DI container
        builder.Services.AddTransient<IMessageService, MessageService>();
        builder.Services.AddSingleton<App>(); // The entry point class for your console logic

        // 3. Build the host
        using var host = builder.Build();

        // 4. Resolve the main application class and execute it
        var app = host.Services.GetRequiredService<App>();
        app.Run();
    }
}