
namespace HeaterSim.Models
{
    public class SimulationConfiguration
    {
        // Configurations for individual sensors
        public Dictionary<int, Func<double, double>> SensorConfigurations { get; set; }

        // Configurations for individual heaters
        public Dictionary<int, Func<int, int>> HeaterConfigurations { get; set; }

        // Configurations for individual fans
        public Dictionary<int, (Func<bool, bool> Configuration, TimeSpan ResponseDelay)> FanConfigurations { get; set; }

        public SimulationConfiguration()
        {
            // Initialize dictionaries
            SensorConfigurations = new Dictionary<int, Func<double, double>>();
            HeaterConfigurations = new Dictionary<int, Func<int, int>>();
            FanConfigurations = new Dictionary<int, (Func<bool, bool>, TimeSpan)>();
        }

        // Example method to apply configurations to a fan
        public async Task ApplyFanConfigurationAsync(FanControl fan)
        {
            if (FanConfigurations.TryGetValue(fan.Id, out var config))
            {
                // Apply delay and configuration
                fan.ResponseDelay = config.ResponseDelay;
                fan.Configuration = config.Configuration;
                await fan.ApplyConfigurationAsync();
            }
        }

        // Example method to apply configurations to a sensor
        public void ApplySensorConfiguration(TemperatureSensor sensor)
        {
            if (SensorConfigurations.TryGetValue(sensor.Id, out var config))
            {
                sensor.Configuration = config;
                sensor.ApplyConfiguration();
            }
        }

        // Example method to apply configurations to a heater
        public void ApplyHeaterConfiguration(HeatingControl heater)
        {
            if (HeaterConfigurations.TryGetValue(heater.Id, out var config))
            {
                heater.Configuration = config;
                heater.ApplyConfiguration();
            }
        }
    }
}

