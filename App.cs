using BeautifulClient.Configuration;
using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api;
using BeautifulClient.UI;
using BeautifulClient.UI.Pages;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace BeautifulClient;

public class App(
    ILogger<App> logger,
    IOptions<MySettings> options,
    IOptions<ApiSettings> apiSettings,
    IApiService apiService)
{
    private MySettings Settings { get; } = options.Value;
    private ApiSettings ApiSettings { get; } = apiSettings.Value;   

    public async Task RunAsync()
    {
        AppActivity("Starting");
        
        try
        {
            PageInitialiser pageInitialiser = new PageInitialiser(apiService);
            await pageInitialiser.RunAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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