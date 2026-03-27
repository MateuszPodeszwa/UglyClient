using HeaterSim.Models;
using Microsoft.AspNetCore.Mvc;
using Version2.Models;
using Version2.Models.Version2.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HeaterSim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorController : ControllerBase
    {
        private readonly ClientStateManager _clientStateManager;

        public SensorController(ClientStateManager clientStateManager)
        {
            _clientStateManager = clientStateManager;
        }

        [HttpGet("sensor1")]
        public IActionResult GetSensor1Temperature()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var sensor = state.Sensors.FirstOrDefault(s => s.Id == 1);
            if (sensor == null)
                return NotFound("Sensor 1 not found");

            Console.WriteLine($"Sensor 1 Temperature: {sensor.CurrentTemperature}");
            return Ok(sensor.CurrentTemperature.ToString("F1")); // Return as a string
        }

        [HttpGet("sensor2")]
        public IActionResult GetSensor2Temperature()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var sensor = state.Sensors.FirstOrDefault(s => s.Id == 2);
            if (sensor == null)
                return NotFound("Sensor 2 not found");

            Console.WriteLine($"Sensor 2 Temperature: {sensor.CurrentTemperature}");
            return Ok((int)Math.Round(sensor.CurrentTemperature + 1)); // Return as an int
        }

        [HttpGet("sensor3")]
        public IActionResult GetSensor3Temperature()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var sensor = state.Sensors.FirstOrDefault(s => s.Id == 3);
            if (sensor == null)
                return NotFound("Sensor 3 not found");

            Console.WriteLine($"Sensor 3 Temperature: {sensor.CurrentTemperature}");
            return Ok((decimal)sensor.CurrentTemperature - 0.25m); // Return as a decimal
        }


        [HttpGet("configurations")]
        public IActionResult GetSensorConfigurations()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var configurations = state.Sensors.Select(s => new SensorConfigDTO
            {
                Id = s.Id,
                LogicDescription = s.Configuration?.Method.Name ?? "Default"
            });

            return Ok(configurations);
        }


        [HttpGet("{sensorId}")]
        public IActionResult GetTemperature(int sensorId)
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var sensor = state.Sensors.FirstOrDefault(s => s.Id == sensorId);
            if (sensor == null)
                return NotFound("Sensor not found");

            return Ok(sensor.CurrentTemperature);
        }
    }

}
