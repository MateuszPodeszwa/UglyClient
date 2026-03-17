using BeautifulClient.Configuration;
using BeautifulClient.Services.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeautifulClient;

public class App(
    IOptions<MySettings> options,
    IOptions<ApiSettings> apiSettings,
    IHardwareApiService hardwareApiService)
{
    private MySettings Settings { get; } = options.Value;
    private ApiSettings ApiSettings { get; } = apiSettings.Value;

    public async Task RunAsync()
    {
        Console.WriteLine("App started running.");
        try
        {
            var sensor1 = await hardwareApiService.GetSensorTemperatureAsync(1);

            Console.WriteLine(sensor1.IsSuccess
                ? $"Sensor 1: {sensor1.Value.Temperature}"
                : $"Sensor 1: {sensor1.Error.Message}");
        }
        catch (Exception)
        {
            Console.WriteLine("Errororororororo");
        }

        Console.WriteLine("App finished running.");
    }
}