using BeautifulClient.Configuration;
using BeautifulClient.Data;
using BeautifulClient.Data.Dto;
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
            Console.WriteLine($"Sensor1: {sensor1.Value.Temperature}");

            ApiResult<SensorData> updatedSensor1 = sensor1;

            Console.WriteLine("Calling Local SetAsync");
            var setTemperature = await apiService.SetHeaterLevelAsync(1, 1);

            Console.WriteLine("Calling Hardware GetAsync");
            var hardwareSensor1 = await apiService.GetSensorTemperatureAsync(5);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine("App finished running.");
    }
}