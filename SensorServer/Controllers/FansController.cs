using HeaterSim.Models;
using Microsoft.AspNetCore.Mvc;
using Version2.Models;
using Version2.Models.Version2.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Version2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FansController : ControllerBase
    {
        private readonly ClientStateManager _clientStateManager;

        public FansController(ClientStateManager clientStateManager)
        {
            _clientStateManager = clientStateManager;
        }

        [HttpGet("{fanId}/state")]
        public IActionResult GetFanState(int fanId)
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var fan = state.Fans.FirstOrDefault(f => f.Id == fanId);
            if (fan == null)
                return NotFound("Fan not found.");

            Console.WriteLine($"Retrieved Fan {fanId} State: {(fan.IsOn ? "On" : "Off")}");

            return Ok(new { Id = fan.Id, IsOn = fan.IsOn });
        }


        [HttpGet("configurations")]
        public IActionResult GetFanConfigurations()
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var configurations = state.Fans.Select(f => new FanConfigDTO
            {
                Id = f.Id,
                State = f.IsOn,
                DelaySeconds = (int)f.ResponseDelay.TotalSeconds
            });

            return Ok(configurations);
        }


        [HttpPost("{fanId}")]
        public IActionResult SetFanState(int fanId, [FromBody] bool isOn)
        {
            var clientId = HttpContext.Items["ClientId"] as string;
            if (string.IsNullOrEmpty(clientId))
                return Unauthorized("Client ID is required.");

            var state = _clientStateManager.GetOrCreateState(clientId);

            var fan = state.Fans.FirstOrDefault(f => f.Id == fanId);
            if (fan == null)
                return NotFound("Fan not found");

            fan.IsOn = isOn;
            return Ok();
        }
    }


}
