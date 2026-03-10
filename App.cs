using BeautifulClient.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(IMessageService messageService, IOptions<MySettings> options, ILogger<App> logger)
{
    private readonly MySettings _settings = options.Value;

    public void Run()
    {
        logger.LogInformation("App started running.");

        try
        {
            messageService.SendMessage(_settings.GreetingMessage);
            logger.LogInformation("Greeting message was processed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "A critical error occurred while sending the message.");
        }
        
        logger.LogInformation("App finished running.");
    }
}