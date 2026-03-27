namespace HeaterSim.Models
{
    public class HeatingControl
    {
        public int Id { get; set; }
        public int Level { get; set; } // Range: 0-5

        // New property added to define custom behavior/configuration for the heater level
        public Func<int, int>? Configuration { get; set; } // Custom behavior based on level

        // Method to apply configuration if it exists
        public void ApplyConfiguration()
        {
            if (Configuration != null)
            {
                Level = Configuration(Level);
            }
        }
    }
}

