using HeaterSim.Models;
using System;

namespace Version2.Seeding
{
    public static class EnvironmentStateSeeder
    {
        public static EnvironmentState SeedEnvironmentState()
        {
            var state = new EnvironmentState();

            // Initialize sensors
            for (int i = 1; i <= 3; i++)
            {
                state.Sensors.Add(new TemperatureSensor
                {
                    Id = i,
                    CurrentTemperature = 17,
                    Configuration = i switch
                    {
                        1 => temp => temp + 1.0,   // Sensor 1 always adds 1 degree
                        2 => temp => temp - 0.5,  // Sensor 2 subtracts 0.5 degrees
                        _ => temp => temp         // Sensor 3 remains unchanged
                    }
                });
            }

            // Initialize heaters
            for (int i = 1; i <= 3; i++)
            {
                state.Heaters.Add(new HeatingControl
                {
                    Id = i,
                    Level = 0,
                    Configuration = i switch
                    {
                        1 => level => level * 2,    // Heater 1 doubles level
                        2 => level => level + 1,   // Heater 2 adds 1 to level
                        _ => level => level        // Heater 3 remains unchanged
                    }
                });
            }

            // Initialize fans
            for (int i = 1; i <= 3; i++)
            {
                state.Fans.Add(new FanControl
                {
                    Id = i,
                    IsOn = false,
                    Configuration = i switch
                    {
                        1 => isOn => !isOn,             // Fan 1 toggles state
                        2 => isOn => true,             // Fan 2 always on
                        _ => isOn => isOn              // Fan 3 remains unchanged
                    },
                    ResponseDelay = i switch
                    {
                        1 => TimeSpan.FromSeconds(2),  // Fan 1 has 2-second delay
                        2 => TimeSpan.FromSeconds(5),  // Fan 2 has 5-second delay
                        _ => TimeSpan.Zero             // Fan 3 no delay
                    }
                });
            }

            return state;
        }
    }
}

