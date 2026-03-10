using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeautifulClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Bind the "MySettings" section from appsettings.json to the MySettings class
        // For debug only, until I figure out how to benefit from it
        builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
        
        // Add Dependencies (DI)
        builder.Services.AddTransient<IMessageService, MessageService>();
        builder.Services.AddSingleton<App>(); // The entry point class for your console logic
        
        using var host = builder.Build();

        // Resolve the main application class (entry point for the app) and execute it
        var app = host.Services.GetRequiredService<App>();
        app.Run();
    }
}