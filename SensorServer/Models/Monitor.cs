using Version2.Models.Version2.Models;

namespace Version2.Models
{
    using HeaterSim.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SimulationMonitorService : BackgroundService
    {
        private readonly ClientStateManager _clientStateManager;
        private readonly TemperatureCalculator _calculator;
        private readonly TimeSpan _maxSimulationDuration = TimeSpan.FromMinutes(30); // Example: 30 minutes
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5); // Adjusted for more frequent checks during debugging

        public SimulationMonitorService(ClientStateManager clientStateManager, TemperatureCalculator calculator)
        {
            _clientStateManager = clientStateManager;
            _calculator = calculator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                foreach (var clientId in _clientStateManager.GetAllClientIds())
                {
                    var state = _clientStateManager.GetOrCreateState(clientId);
                    Console.WriteLine($"Processing environment for client: {clientId}");

                    // Check if a simulation is running
                    if (state.IsSimulationRunning && state.SimulationStartTime.HasValue)
                    {
                        var elapsed = now - state.SimulationStartTime.Value;

                        // Debug: Log elapsed time
                        Console.WriteLine($"Elapsed time for client {clientId}: {elapsed.TotalMinutes:F2} minutes");

                        // Terminate if the simulation exceeds max duration
                        if (elapsed > _maxSimulationDuration)
                        {
                            state.SimulationStartTime = null;
                            Console.WriteLine($"Simulation for client {clientId} terminated due to timeout.");
                            continue;
                        }


                        // Apply configurations before updating the environment
                        await state.ApplyConfigurationsAsync(); // NEW: Apply configurations

                        // Update environment temperature
                        UpdateEnvironment(state, elapsed.TotalMinutes);
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private void UpdateEnvironment(EnvironmentState state, double elapsedMinutes)
        {
            Console.WriteLine($"Updating environment for client. Last updated: {state.LastUpdated}");

            foreach (var sensor in state.Sensors)
            {
                Console.WriteLine($"Sensor {sensor.Id} temperature before update: {sensor.CurrentTemperature}");

                // Calculate the net temperature change
                double netChange = _calculator.CalculateNetTemperatureChange(state.Heaters, state.Fans, elapsedMinutes);

                // Debug: Log heater and fan effects
                double heaterEffect = _calculator.CalculateHeaterEffect(state.Heaters, elapsedMinutes);
                double fanEffect = _calculator.CalculateFanEffect(state.Fans, elapsedMinutes);

                Console.WriteLine($"Heater Effect: {heaterEffect}, Fan Effect: {fanEffect}");
                Console.WriteLine($"Net Temperature Change: {netChange}");

                // Apply the temperature change
                sensor.CurrentTemperature += netChange;
                Console.WriteLine($"Sensor {sensor.Id} temperature after update: {sensor.CurrentTemperature}");
            }

            state.LastUpdated = DateTime.Now; // Update the last updated timestamp
        }
    }


}
