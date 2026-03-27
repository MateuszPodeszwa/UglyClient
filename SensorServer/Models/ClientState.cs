namespace Version2.Models
{
    using HeaterSim.Models;
    using System.Collections.Concurrent;


    namespace Version2.Models
    {
        using HeaterSim.Models;
        using System.Collections.Concurrent;

        using System;
        using System.Collections.Concurrent;
        using System.Collections.Generic;

        public class ClientStateManager
        {
            private class ClientState
            {
                public EnvironmentState State { get; set; }
                public DateTime LastAccessed { get; set; }
            }

            private readonly ConcurrentDictionary<string, ClientState> _clientStates = new();
            private readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(30); // Example: 30-minute expiration
            private readonly TimeSpan _maxSimulationDuration = TimeSpan.FromMinutes(30); // Example: max simulation duration
            private readonly ConcurrentDictionary<string, int> _activeSimulations = new();
            private readonly int _maxSimulationsPerClient = 3;

            ///// <summary>
            ///// Gets or creates the state for the given client ID.
            ///// </summary>
            //public EnvironmentState GetOrCreateState(string clientId)
            //{
            //    var clientState = _clientStates.GetOrAdd(clientId, _ =>
            //    {
            //        var newState = new ClientState
            //        {
            //            State = new EnvironmentState(), // Initialisation is handled in EnvironmentState
            //            LastAccessed = DateTime.Now
            //        };

            //        return newState;
            //    });

            //    clientState.LastAccessed = DateTime.Now; // Update access time
            //    return clientState.State;
            //}


            public EnvironmentState GetOrCreateState(string clientId)
            {
                Console.WriteLine($"Retrieving state for clientId={clientId}");

                var clientState = _clientStates.GetOrAdd(clientId, _ =>
                {
                    Console.WriteLine($"Creating new state for clientId={clientId}");
                    return new ClientState
                    {
                        State = new EnvironmentState(),
                        LastAccessed = DateTime.Now
                    };
                });

                clientState.LastAccessed = DateTime.Now;

                // Log current state to ensure updates persist
                Console.WriteLine("Current State:");
                foreach (var fan in clientState.State.Fans)
                    Console.WriteLine($"  Fan {fan.Id}: IsOn={fan.IsOn}");
                foreach (var heater in clientState.State.Heaters)
                    Console.WriteLine($"  Heater {heater.Id}: Level={heater.Level}");
                foreach (var sensor in clientState.State.Sensors)
                    Console.WriteLine($"  Sensor {sensor.Id}: Temperature={sensor.CurrentTemperature}");

                return clientState.State;
            }

            /// <summary>
            /// Checks if the client can start a new simulation.
            /// </summary>
            public bool CanStartSimulation(string clientId)
            {
                _activeSimulations.TryGetValue(clientId, out var count);
                return count < _maxSimulationsPerClient;
            }

            /// <summary>
            /// Starts a simulation for the given client ID.
            /// </summary>
            public void StartSimulation(string clientId)
            {
                _activeSimulations.AddOrUpdate(clientId, 1, (_, count) => count + 1);

                if (_clientStates.TryGetValue(clientId, out var clientState))
                {
                    clientState.State.SimulationStartTime = DateTime.Now; // Use EnvironmentState's SimulationStartTime
                }
            }

            /// <summary>
            /// Stops a simulation for the given client ID.
            /// </summary>
            public void StopSimulation(string clientId)
            {
                _activeSimulations.AddOrUpdate(clientId, 0, (_, count) => Math.Max(0, count - 1));

                if (_clientStates.TryGetValue(clientId, out var clientState))
                {
                    clientState.State.SimulationStartTime = null; // Clear EnvironmentState's SimulationStartTime
                }
            }

            /// <summary>
            /// Removes the state for the given client ID.
            /// </summary>
            public void RemoveState(string clientId)
            {
                _clientStates.TryRemove(clientId, out _);
            }

            /// <summary>
            /// Cleans up expired states that haven't been accessed within the expiration time.
            /// </summary>
            public void CleanupExpiredStates()
            {
                var now = DateTime.Now;

                foreach (var kvp in _clientStates)
                {
                    if (now - kvp.Value.LastAccessed > _expirationTime)
                    {
                        _clientStates.TryRemove(kvp.Key, out _);
                    }
                }
            }

            public void ResetClientState(string clientId)
            {
                if (_clientStates.TryGetValue(clientId, out var clientState))
                {
                    clientState.State.ResetState();
                    Console.WriteLine($"Client state for clientId={clientId} has been reset.");
                }
                else
                {
                    Console.WriteLine($"No state found for clientId={clientId}. Cannot reset.");
                }
            }

            /// <summary>
            /// Terminates simulations that exceed the maximum allowed duration.
            /// </summary>
            public void TerminateLongRunningSimulations()
            {
                var now = DateTime.Now;

                foreach (var kvp in _clientStates)
                {
                    var clientId = kvp.Key;
                    var clientState = kvp.Value;

                    if (clientState.State.SimulationStartTime.HasValue) // Check EnvironmentState's SimulationStartTime
                    {
                        var elapsed = now - clientState.State.SimulationStartTime.Value;

                        if (elapsed > _maxSimulationDuration)
                        {
                            clientState.State.SimulationStartTime = null; // Stop the simulation
                            _activeSimulations.AddOrUpdate(clientId, 0, (_, count) => Math.Max(0, count - 1));

                            Console.WriteLine($"Simulation for client {clientId} terminated after exceeding {_maxSimulationDuration.TotalMinutes} minutes.");
                        }
                    }
                }
            }

            /// <summary>
            /// Applies configurations to all client states.
            /// </summary>
            public async Task ApplyConfigurationsToAllClientsAsync()
            {
                foreach (var kvp in _clientStates)
                {
                    var clientState = kvp.Value;
                    await clientState.State.ApplyConfigurationsAsync(); // NEW: Apply configurations to each state
                }
            }


            /// <summary>
            /// Gets all client IDs currently in the system.
            /// </summary>
            public IEnumerable<string> GetAllClientIds()
            {
                return _clientStates.Keys;
            }
        }

    }

}
