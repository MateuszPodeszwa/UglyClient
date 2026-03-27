using HeaterSim.Models;
using Microsoft.AspNetCore.Mvc;
using Version2.Models;
using Version2.Models.Version2.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HeaterSim.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvoController : ControllerBase
    {
        private readonly ClientStateManager _clientStateManager;
        private readonly TemperatureCalculator _calculator;

        public EnvoController(ClientStateManager clientStateManager, TemperatureCalculator calculator)
        {
            _clientStateManager = clientStateManager;
            _calculator = calculator;
        }

        [HttpPost("reset")]
        public IActionResult ResetClientState()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            _clientStateManager.ResetClientState(clientId);
            return Ok("Client state has been reset.");
        }

        [HttpPost]
        public IActionResult UpdateEnvironment()
        {
            var clientId = HttpContext.Items["ClientId"] as string; // Extract client ID
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var elapsedMinutes = (DateTime.Now - state.LastUpdated).TotalMinutes;
            if (elapsedMinutes < 1)
                return BadRequest("Wait longer before updating.");

            foreach (var sensor in state.Sensors)
            {
                double netTemperatureChange = _calculator.CalculateNetTemperatureChange(
                    state.Heaters,
                    state.Fans,
                    elapsedMinutes);

                sensor.CurrentTemperature += netTemperatureChange;
            }

            state.LastUpdated = DateTime.Now;

            return Ok("Environment updated.");
        }
    }

}
