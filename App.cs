using BeautifulClient.Configuration;
using BeautifulClient.Data.Objects;
using BeautifulClient.Services.Api;
using BeautifulClient.UI;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        logger.LogInformation($"""
                               
                               ----------------------------------------
                               Starting {nameof(App)}
                               ----------------------------------------
                               """);
        
        try
        {
            ApiResult<FanData> fanData1 = await apiService.GetFanDataAsync(2);

            if (fanData1.IsSuccess)
            {
                Console.WriteLine($"Fan data retrieved {fanData1.Value}.");
                
                var fanData = fanData1.Value;
                
                fanData.Status = true;

                ApiResult updateResult = await fanData.SaveOnChangesAsync();

                if (updateResult.IsSuccess)
                {
                    Console.WriteLine($"Fan data saved {fanData1.Value}.");
                    
                    ApiResult<FanData> fanData1_2 = await apiService.GetFanDataAsync(2);

                    if (fanData1_2.IsSuccess)
                    {
                        Console.WriteLine($"Fan data saved-verified? {fanData1_2.Value}.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        logger.LogInformation($"""
                               
                               ----------------------------------------
                               Ending {nameof(App)}
                               ----------------------------------------
                               """);
    }
}