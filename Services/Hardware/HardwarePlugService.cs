using System.Globalization;

namespace BeautifulClient.Services.Hardware;

// Simulates temperature sensor that is directly plugged into the system
public static class HardwarePlugService
{
    private static readonly int[] ValidSensors = [4, 5, 6];

    // Simulates temperature sensor that returns string as a temperature
    public static async Task<string?> GetTemperatureStringAsync(int sensorId) 
    {
        // Simulates the latency of communicating with hardware (e.g., 50ms to 200ms)
        await Task.Delay(Random.Shared.Next(50, 200));
        
        return ValidSensors.Contains(sensorId) 
            ? Random.Shared.NextDouble().ToString(CultureInfo.InvariantCulture) 
            : null;
    }
    
    // Simulates temperature sensor that returns float as a temperature
    public static async Task<float?> GetTemperatureFloatAsync(int sensorId) 
    {
        await Task.Delay(Random.Shared.Next(50, 200));

        return ValidSensors.Contains(sensorId)
            ? Random.Shared.NextSingle()
            : null;
    }
    
    // Simulates temperature sensor that returns integer as a temperature
    public static async Task<int?> GetTemperatureIntAsync(int sensorId) 
    {
        await Task.Delay(Random.Shared.Next(50, 200));

        return ValidSensors.Contains(sensorId)
            ? Random.Shared.Next()
            : null;
    }
}