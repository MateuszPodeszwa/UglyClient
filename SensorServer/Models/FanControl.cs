namespace HeaterSim.Models
{
    public class FanControl
    {
        public int Id { get; set; }
        public bool IsOn { get; set; } // True for ON, False for OFF

        // New property added to define custom behavior/configuration for the fan state
        public Func<bool, bool>? Configuration { get; set; } // Custom behavior based on current state

        // New property: Simulate a delay in fan response
        public TimeSpan ResponseDelay { get; set; } = TimeSpan.Zero; // Default: No delay

        // Method to apply configuration if it exists
        public async Task ApplyConfigurationAsync()
        {
            if (Configuration != null)
            {
                // Simulate the response delay
                await Task.Delay(ResponseDelay);

                // Apply the configuration
                IsOn = Configuration(IsOn);
            }
        }
    }
}

