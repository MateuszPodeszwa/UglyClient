using BeautifulClient.Configuration;
using BeautifulClient.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(
    IOptions<MySettings> options,
    ILogger<App> logger,
    IOptions<ApiSettings> apiSettings,
    IHardwareApiService hardwareApiService)
{
    private MySettings Settings { get; } = options.Value;
    private ApiSettings ApiSettings { get; } = apiSettings.Value;

    public async Task RunAsync()
    {
        logger.LogInformation("App started running.");

        try
        {
            var sensor1 = await hardwareApiService.GetSensorTemperatureAsync(1);
            Console.WriteLine(sensor1);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while sending the message.");
        }
        
        logger.LogInformation("App finished running.");
    }
}