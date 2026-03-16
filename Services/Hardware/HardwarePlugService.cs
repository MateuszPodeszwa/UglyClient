namespace BeautifulClient.Services.Hardware;

// Simulates temperature sensor that is directly plugged into the system
public class HardwarePlugService
{
    public static double GetTemperature() => new Random().NextDouble();
}