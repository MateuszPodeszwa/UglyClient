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

// TODO:
// Create a wrapper around existing API architecture,
// creating a method that would automatically update sensors by n * time;
// This will leverage that each IData object's required Id will always be assigned
// to the correct (corresponding) sensor, allowing it to call .Update()

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
            HomePage homePage = new HomePage(apiService);
            await homePage.RunAsync();
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