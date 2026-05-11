
using HeaterSim.Models;


namespace Version2.Models
{
    public class EnvironmentUpdater : BackgroundService
    {
        private readonly EnvironmentState _state;
        private readonly TemperatureCalculator _calculator;

        public EnvironmentUpdater(EnvironmentState state, TemperatureCalculator calculator)
        {
            _state = state;
            _calculator = calculator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UpdateEnvironmentState();
                await Task.Delay(1000, stoppingToken); // Wait 1 second before next update
            }
        }

        private void UpdateEnvironmentState()
        {
            var elapsedSeconds = (DateTime.Now - _state.LastUpdated).TotalSeconds;

            // NEW: Adjust sensor temperatures dynamically based on configurations
            _calculator.AdjustSensorTemperatures(_state.Sensors);

            foreach (var sensor in _state.Sensors)
            {
                // Calculate net temperature change including dynamic heater and fan effects
                double netTemperatureChange = _calculator.CalculateNetTemperatureChange(
                    _state.Heaters,
                    _state.Fans,
                    elapsedSeconds / 60); // Convert seconds to minutes

                // Apply the calculated change
                sensor.CurrentTemperature += netTemperatureChange;

                Console.WriteLine($"Sensor {sensor.Id} temperature updated to: {sensor.CurrentTemperature}");
            }

            // Update the timestamp
            _state.LastUpdated = DateTime.Now;
        }
    }
}

