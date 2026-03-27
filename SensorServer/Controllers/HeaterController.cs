using HeaterSim.Models;
using Microsoft.AspNetCore.Mvc;
using Version2.Models;
using Version2.Models.Version2.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HeaterSim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeatController : ControllerBase
    {
        private readonly ClientStateManager _clientStateManager;

        public HeatController(ClientStateManager clientStateManager)
        {
            _clientStateManager = clientStateManager;
        }

        [HttpGet("{heaterId}/level")]
        public IActionResult GetHeaterLevel(int heaterId)
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var heater = state.Heaters.FirstOrDefault(h => h.Id == heaterId);
            if (heater == null)
                return NotFound("Heater not found.");

            Console.WriteLine($"Retrieved Heater {heaterId} Level: {heater.Level}");

            return Ok(heater.Level);
        }


        // NEW: Endpoint to expose heater configurations
        [HttpGet("configurations")]
        public IActionResult GetHeaterConfigurations()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var configurations = state.Heaters.Select(h => new HeaterConfigDTO
            {
                Id = h.Id,
                CurrentLevel = h.Level // Exposes current heater level
            });

            return Ok(configurations);
        }


        [HttpPost("{heaterId}")]
        public IActionResult SetHeaterLevel(int heaterId, [FromBody] int level)
        {
            if (level < 0 || level > 5)
                return BadRequest("Level must be between 0 and 5");

            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var heater = state.Heaters.FirstOrDefault(h => h.Id == heaterId);
            if (heater == null)
                return NotFound("Heater not found");

            Console.WriteLine($"Setting Heater {heaterId} level from {heater.Level} to {level}");

            heater.Level = level;
            return Ok();
        }
    }
}
    



