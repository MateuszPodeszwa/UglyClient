namespace Version2.Models
{
    public class SystemStateDTO
    {
        public List<HeaterDTO> Heaters { get; set; }
        public List<FanDTO> Fans { get; set; }
    }

    public class HeaterDTO
    {
        public int HeaterId { get; set; }
        public int Level { get; set; } // Heater level (0-5)
    }

    public class FanDTO
    {
        public int FanId { get; set; }
        public bool IsOn { get; set; } // Fan on/off state
    }

    // DTO for sensor configurations
    public class SensorConfigDTO
    {
        public int Id { get; set; } // Sensor ID
        public string LogicDescription { get; set; } // Description of the adjustment logic
    }

    // DTO for fan configurations
    public class FanConfigDTO
    {
        public int Id { get; set; } // Fan ID
        public bool State { get; set; }
        public int DelaySeconds { get; set; } // Response delay in seconds
    }

    // DTO for heater configurations
    public class HeaterConfigDTO
    {
        public int Id { get; set; } // Heater ID
        public int CurrentLevel { get; set; } // Current level of the heater
    }

}
