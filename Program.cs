#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
global using celc = BeautifulClient.Data.Structs.Temperature.Celcius;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

using System.Reflection;
using BeautifulClient.Configuration;
using BeautifulClient.Services.Api;
using BeautifulClient.Services.Api.Adapters;
using BeautifulClient.UI;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Commands;
using BeautifulClient.UI.Layouts;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.UI.Rendering;
using BeautifulClient.Utilities;
using BeautifulClient.Utilities.Extensions;
using BeautifulClient.Utilities.Pipelines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace BeautifulClient;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        
        // Development Env., which forces User Secrets to load
        builder.Environment.EnvironmentName = "Development";
        
        // Explicitly force the configuration to load project's User Secrets
        builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
        
        // Configure Serilog and replace the default .NET logger
        // Logs are saved in the bin/Debug/net10.0/logs
        builder.Services.AddSerilog(config =>
        {
            var uiSink = new SerilogQueSink();
            config
                .ReadFrom.Configuration(builder.Configuration) // Read the configuration from appsettings.json file
                .WriteTo.Sink(uiSink);
        });

        // Bind the "MySettings" section from appsettings.json to the MySettings class
        // For debug only, until I figure out how to benefit from it
        builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));
        
        // Bind ApiSettings (BaseUrl from JSON, ApiKey from User Secrets)
        builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

        // Register the Typed Client and configure its default behaviour
        builder.Services.AddHttpClient<RemoteAdapter>((serviceProvider, client) =>
        {
            // Retrieve the merged settings from the DI container
            var settings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;

            // Set the base URL for all requests made by this client
            client.BaseAddress = new Uri(settings.BaseUrl);

            // Add the API key as a default header
            client.DefaultRequestHeaders.Add("X-Api-Key", settings.ApiKey);
        }).AddPolicyHandler((sp, request) => 
        {
            var logger = sp.GetRequiredService<ILogger<HttpPolicies>>();
            return HttpPolicies.GetRetryPolicy(logger);
        }); // Manually inject ILogger into static HttpPolicies.GetRetryPolicy()
        
        // Add Dependencies (DI)
        builder.Services.AddTransient<IMessageService, MessageService>();
        builder.Services.AddSingleton<App>(); // The entry point class for the console logic
        builder.Services.AddSingleton<ApiResultPipeline>();
        builder.Services.AddSingleton<ObjectSetterPipeline>();
        builder.Services.AddTransient<LocalAdapter>();
        builder.Services.AddTransient<IApiService, UniversalApiFacade>();
        builder.Services.AddSingleton<ConsoleHost>();
        builder.Services.AddSingleton<SerilogQueSink>();
        
        builder.Services.Scan(scan => scan
            // Look in the assembly where HomePageController lives
            .FromAssemblyOf<HomePageController>()
            // Find any class that implements our IRouter interface
            .AddClasses(classes => classes.AssignableTo<IRouter>())
            // Register them as their concrete types (e.g. HomePageController)
            .AsSelf() 
            .WithTransientLifetime());
        
        // Add HomePage with UserDashboardViewModel and apply MainLayout
        builder.AddPageDecorator<UserDashboardModel, HomePage, MainLayout<UserDashboardModel>>(null);
        
        using var host = builder.Build();

        // Resolve the main application class (entry point for the app) and execute it
        var app = host.Services.GetRequiredService<App>();
        await app.RunAsync();
    }
}