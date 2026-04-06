using BeautifulClient.Configuration;
using BeautifulClient.UI;
using BeautifulClient.UI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(
    ILogger<App> logger,
    IOptions<MySettings> options,
    ConsoleHost consoleHost)
{
    private MySettings MySettings { get; } = options.Value;

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