using BeautifulClient.Configuration;
using BeautifulClient.Data;
using BeautifulClient.Data.Objects;
using BeautifulClient.Extensions;
using BeautifulClient.Services.Api;
using BeautifulClient.Utilities.ErrorHandler;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

// TODO:
// Create a wrapper around existing API architecture,
// creating a method that would automatically update sensors by n * time;
// This will leverage that each IData object's required Id will always be assigned
// to the correct (corresponding) sensor, allowing it to call .Update()

public class App(
    IOptions<MySettings> options,
    IOptions<ApiSettings> apiSettings,
    IApiService apiService)
{
    private MySettings Settings { get; } = options.Value;
    private ApiSettings ApiSettings { get; } = apiSettings.Value;   

    public async Task RunAsync()
    {
        Console.WriteLine("App started running.");
        try
        {
            ApiResult<SensorData> sensor1 = await apiService.GetSensorTemperatureAsync(1);
            SensorData sensor1Data = sensor1.Value;
            
            Console.WriteLine($"Initial {nameof(sensor1Data)} modified? {sensor1Data.IsModified}, value: {sensor1Data.Temperature}");
            
            sensor1Data.Temperature = 21;
            Console.WriteLine($"Is {nameof(sensor1Data)} modified? {sensor1Data.IsModified}, value: {sensor1Data.Temperature}");

            await sensor1Data.SaveOnChangesAsync();
            
            // sensor1.Value.Temperature = 7;
            // Console.WriteLine($"Is {nameof(sensor1Data)} modified? {sensor1Data.IsModified}, value: {sensor1Data.Temperature}");
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine("App finished running.");
    }
}