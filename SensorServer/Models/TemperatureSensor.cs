namespace HeaterSim.Models
{
    public class TemperatureSensor
    {
        public int Id { get; set; }
        public double CurrentTemperature { get; set; }

        // New property added to define custom behavior/configuration for the sensor
        public Func<double, double>? Configuration { get; set; } // Custom behavior based on current temperature

        // Method to apply configuration if it exists
        public void ApplyConfiguration()
        {
            if (Configuration != null)
            {
                CurrentTemperature = Configuration(CurrentTemperature);
            }
        }
    }
}

