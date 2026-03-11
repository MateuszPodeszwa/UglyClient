using System.Globalization;
using BeautifulClient.Configuration;
using BeautifulClient.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(
    IMessageService messageService,
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
            messageService.SendMessage(Settings.GreetingMessage);
            messageService.SendMessage(ApiSettings.ApiKey);
            messageService.SendMessage(ApiSettings.BaseUrl);
            logger.LogInformation("Greeting message was processed successfully.");

            var sensor1 = await hardwareApiService.GetSensorTemperatureAsync(1);
            messageService.SendMessage(Convert.ToString(sensor1, CultureInfo.CurrentCulture));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while sending the message.");
        }
        
        logger.LogInformation("App finished running.");
    }
}