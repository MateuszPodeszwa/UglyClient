using global::HeaterSim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Version2.Models;
using Version2.Models.Version2.Models;

namespace Version2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemStateController : ControllerBase
    {
        private readonly ClientStateManager _clientStateManager;

        public SystemStateController(ClientStateManager clientStateManager)
        {
            _clientStateManager = clientStateManager;
        }

        [HttpGet("fullstate")]
        public IActionResult GetFullSystemState()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var fullState = new
            {
                SystemState = new SystemStateDTO
                {
                    Heaters = state.Heaters.Select(h => new HeaterDTO
                    {
                        HeaterId = h.Id,
                        Level = h.Level
                    }).ToList(),
                    Fans = state.Fans.Select(f => new FanDTO
                    {
                        FanId = f.Id,
                        IsOn = f.IsOn
                    }).ToList()
                },
                SensorConfigurations = state.Sensors.Select(s => new SensorConfigDTO
                {
                    Id = s.Id,
                    LogicDescription = s.Configuration?.Method.Name ?? "Default"
                }).ToList(),
                FanConfigurations = state.Fans.Select(f => new FanConfigDTO
                {
                    Id = f.Id,
                    DelaySeconds = (int)f.ResponseDelay.TotalSeconds
                }).ToList()
            };

            return Ok(fullState);
        }


        [HttpGet]
        public IActionResult GetSystemState()
        {
            // Extract the client ID from the HttpContext
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            // Retrieve the client-specific environment state
            var state = _clientStateManager.GetOrCreateState(clientId);

            // Map environment state to DTO
            var systemState = new SystemStateDTO
            {
                Heaters = state.Heaters.Select(h => new HeaterDTO
                {
                    HeaterId = h.Id,
                    Level = h.Level
                }).ToList(),

                Fans = state.Fans.Select(f => new FanDTO
                {
                    FanId = f.Id,
                    IsOn = f.IsOn
                }).ToList()
            };

            return Ok(systemState);
        }
    }

}

