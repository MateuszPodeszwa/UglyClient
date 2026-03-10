using BeautifulClient.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BeautifulClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Configure Serilog and replace the default .NET logger
        // Logs are saved in the bin/Debug/net10.0/logs
        builder.Services.AddSerilog(config => 
        {
            config.ReadFrom.Configuration(builder.Configuration); // Read the configuration from appsettings.json file
        });

        // Bind the "MySettings" section from appsettings.json to the MySettings class
        // For debug only, until I figure out how to benefit from it
        builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
        
        // Add Dependencies (DI)
        builder.Services.AddTransient<IMessageService, MessageService>();
        builder.Services.AddSingleton<App>(); // The entry point class for the console logic
        
        using var host = builder.Build();

        // Resolve the main application class (entry point for the app) and execute it
        var app = host.Services.GetRequiredService<App>();
        app.Run();
    }
}