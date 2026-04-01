using BeautifulClient.Configuration;
using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api;
using BeautifulClient.UI;
using BeautifulClient.UI.Controllers;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace BeautifulClient;

public class App(
    ILogger<App> logger,
    IOptions<MySettings> options,
    IOptions<ApiSettings> apiSettings,
    IApiService apiService,
    IServiceProvider serviceProvider,
    ConsoleHost consoleHost)
{
    private MySettings MySettings { get; } = options.Value;
    private ApiSettings ApiSettings { get; } = apiSettings.Value;   

    public async Task RunAsync()
    {
        AppActivity("Starting");
        
        try
        {
            await consoleHost.Host<HomePageController>(MySettings);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            logger.LogError(e.ToString());
        }

        AppActivity("Ending");
        return;

        void AppActivity(string activity)
        {
            logger.LogInformation
            ($"""
                    
              ----------------------------------------
              {activity} {nameof(App)}
              ----------------------------------------
              """);
        }
    }
}