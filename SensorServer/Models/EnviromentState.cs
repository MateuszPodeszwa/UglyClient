
namespace HeaterSim.Models
{
    public class EnvironmentState
    {
        public List<TemperatureSensor> Sensors { get; set; }
        public List<HeatingControl> Heaters { get; set; }
        public List<FanControl> Fans { get; set; }
        public SimulationConfiguration Configuration { get; set; } // Added for centralized configurations
        public DateTime LastUpdated { get; set; }
        public DateTime? SimulationStartTime { get; set; }

        public EnvironmentState()
        {
            Sensors = new List<TemperatureSensor>();
            Heaters = new List<HeatingControl>();
            Fans = new List<FanControl>();
            Configuration = new SimulationConfiguration(); // Initialise the configuration
            LastUpdated = DateTime.Now;

            // Automatically start a session
            SimulationStartTime = DateTime.Now;

            //// Initialize default sensors
            for (int i = 1; i <= 3; i++)
            {
                Sensors.Add(new TemperatureSensor
                {
                    Id = i,
                    CurrentTemperature = 18.0 // Default temperature
                });
            }

            //// Initialize default heaters
            for (int i = 1; i <= 3; i++)
            {
                Heaters.Add(new HeatingControl
                {
                    Id = i,
                    Level = 0 // Default level
                });
            }

            //// Initialize default fans
            for (int i = 1; i <= 3; i++)
            {
                Fans.Add(new FanControl
                {
                    Id = i,
                    IsOn = false // Default state
                });
            }

            Console.WriteLine($"Simulation started automatically at {SimulationStartTime}");
        }

        public void ResetState()
        {
            // Reset sensors to 17°C
            foreach (var sensor in Sensors)
            {
                sensor.CurrentTemperature = 17.0;
            }

            // Turn off all fans
            foreach (var fan in Fans)
            {
                fan.IsOn = false;
            }

            // Set all heaters to level 0
            foreach (var heater in Heaters)
            {
                heater.Level = 0;
            }

            Console.WriteLine("Environment state has been reset: Fans off, Heaters off, Sensors to 17°C.");
        }


        public bool IsSimulationRunning => SimulationStartTime.HasValue;

        // New method to apply configurations to all components
        public async Task ApplyConfigurationsAsync()
        {
            foreach (var sensor in Sensors)
            {
                Configuration.ApplySensorConfiguration(sensor);
            }

            foreach (var heater in Heaters)
            {
                Configuration.ApplyHeaterConfiguration(heater);
            }

            foreach (var fan in Fans)
            {
                await Configuration.ApplyFanConfigurationAsync(fan);
            }
        }
    }
}

