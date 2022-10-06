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
            Console.WriteLine($"Received state update for {state.Key}");
            
            var databaseGrain = _client.GetGrain<IDatabaseGrain>(state.Key);
            await databaseGrain.UpdateState(state);
            
            return Ok();
        }
    }
}
