using BeautifulClient.Configuration;
using BeautifulClient.Services;
using BeautifulClient.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(
    IMessageService messageService,
    IOptions<MySettings> options,
    ILogger<App> logger,
    IOptions<ApiSettings> apiSettings,
    IApiService apiService)
{
    private readonly MySettings _settings = options.Value;
    private readonly ApiSettings _apiSettings = apiSettings.Value;

    public void Run()
    {
        logger.LogInformation("App started running.");

        try
        {
            messageService.SendMessage(_settings.GreetingMessage);
            messageService.SendMessage(_apiSettings.ApiKey);
            messageService.SendMessage(_apiSettings.BaseUrl);
            logger.LogInformation("Greeting message was processed successfully.");
            messageService.SendMessage(apiService.GetAsync().Result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while sending the message.");
        }
        
        logger.LogInformation("App finished running.");
    }
}