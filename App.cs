using BeautifulClient.Configuration;
using BeautifulClient.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

// TODO:
// Create a wrapper around existing API architecture,
// creating a method that would automatically update sensors by n * time;
// This will leverage that each IData object's required Id will 

public class App(
    IOptions<MySettings> options,
    IOptions<ApiSettings> apiSettings,
    IHardwareApiService apiService)
{
    private MySettings Settings { get; } = options.Value;
    private ApiSettings ApiSettings { get; } = apiSettings.Value;

    public async Task RunAsync()
    {
        Console.WriteLine("App started running.");
        try
        {
            var sensor1 = await apiService.GetSensorTemperatureAsync(5);
            Console.WriteLine($"Sensor1: {sensor1.Value.Temperature}");
            
            //Console.WriteLine("Calling Local SetAsync");
            //var setTemperature = await apiService.SetHeaterLevelAsync(1, 1);

            //Console.WriteLine("Calling Hardware GetAsync");
            //var hardwareSensor1 = await apiService.GetSensorTemperatureAsync(5);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.WriteLine("App finished running.");
    }
}