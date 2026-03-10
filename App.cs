using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(IMessageService messageService, IOptions<MySettings> options, ILogger<App> logger)
{
    private readonly MySettings _settings = options.Value;

    public void Run()
    {
        // Use the logger to record application flow
        logger.LogInformation("App started running.");

        try
        {
            messageService.SendMessage(_settings.GreetingMessage);
            logger.LogInformation("Greeting message was processed successfully.");
        }
        catch (Exception ex)
        {
            // LogError can take the Exception object directly to print the stack trace
            logger.LogError(ex, "A critical error occurred while sending the message.");
        }
        
        logger.LogInformation("App finished running.");
    }
}