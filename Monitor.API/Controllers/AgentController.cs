using Microsoft.AspNetCore.Mvc;
using Monitor.Grains;
using Monitor.Shared;
using Orleans;

namespace Monitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IClusterClient _client;

        public AgentController(IClusterClient client)
        {
            _client = client;
        }

        [HttpPut]
        public async Task<IActionResult> NotifyDatabaseState([FromBody] DatabaseState state)
        {
            var key = state.GetKey();
            Console.WriteLine($"Received state update for {key}");
            
            var databaseGrain = _client.GetGrain<IDatabaseGrain>(key);
            await databaseGrain.UpdateState(state);
            
            return Ok();
        }
    }
}
