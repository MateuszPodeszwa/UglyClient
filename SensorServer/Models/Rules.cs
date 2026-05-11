using HeaterSim.Controllers;
using HeaterSim.Models;

namespace Version2.Models
{
    public class TemperatureCalculator
    {
        private const double HeatFactor = 0.1;  // Heater effect per level per minute
        private const double CoolingFactor = -0.5; // Cooling effect per fan per minute

        // NEW: Method to calculate dynamic heat factor based on heater configuration
        private double CalculateDynamicHeatFactor(HeatingControl heater, double elapsedMinutes)
        {
            return heater.Level * elapsedMinutes * HeatFactor;
        }

        // NEW: Method to calculate fan effect considering response delay
        private double CalculateDynamicFanEffect(FanControl fan, double elapsedMinutes)
        {
            return fan.IsOn ? elapsedMinutes * CoolingFactor : 0;
        }

        public double CalculateHeaterEffect(IEnumerable<HeatingControl> heaters, double elapsedMinutes)
        {
            // Updated to include dynamic configuration logic for heaters
            double effect = heaters.Sum(h => CalculateDynamicHeatFactor(h, elapsedMinutes));
            Console.WriteLine($"Heater Effect: {effect} (Elapsed Minutes: {elapsedMinutes})");
            return effect;
        }

        public double CalculateFanEffect(IEnumerable<FanControl> fans, double elapsedMinutes)
        {
            // Updated to include dynamic configuration logic for fans
            return fans.Sum(f => CalculateDynamicFanEffect(f, elapsedMinutes));
        }

        public double CalculateNetTemperatureChange(IEnumerable<HeatingControl> heaters, IEnumerable<FanControl> fans, double elapsedMinutes)
        {
            double heaterEffect = CalculateHeaterEffect(heaters, elapsedMinutes);
            double fanEffect = CalculateFanEffect(fans, elapsedMinutes);

            return heaterEffect + fanEffect;
        }

        // NEW: Method to calculate sensor adjustments
        public void AdjustSensorTemperatures(IEnumerable<TemperatureSensor> sensors)
        {
            foreach (var sensor in sensors)
            {
                sensor.ApplyConfiguration(); // Applies sensor-specific configuration logic
                Console.WriteLine($"Sensor {sensor.Id} adjusted to: {sensor.CurrentTemperature}");
            }
        }
    }
}

