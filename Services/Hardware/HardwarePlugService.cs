using System.Globalization;
using System.Net.Sockets;

namespace BeautifulClient.Services.Hardware;

// Simulates temperature sensor that is directly plugged into the system
public static class HardwarePlugService
{
    // Simulates temperature sensor that returns string as a temperature
    public static string GetTemperatureString(int sensorId) {
        int[] validSensors = [4, 5, 6];
        
        return validSensors.Contains(sensorId) 
            ? Random.Shared.NextDouble().ToString(CultureInfo.InvariantCulture) 
            : throw new SocketException(404, "There is no such sensor");
    }
    
    // Simulates temperature sensor that returns float as a temperature
    public static float GetTemperatureFloat(int sensorId) {
        int[] validSensors = [4, 5, 6];
        
        return validSensors.Contains(sensorId) 
            ? Random.Shared.NextSingle()
            : throw new SocketException(404, "There is no such sensor");
    }
    
    // Simulates temperature sensor that returns integer as a temperature
    public static int GetTemperatureInt(int sensorId) {
        int[] validSensors = [4, 5, 6];
        
        return validSensors.Contains(sensorId) 
            ? Random.Shared.Next()
            : throw new SocketException(404, "There is no such sensor");
    }
}