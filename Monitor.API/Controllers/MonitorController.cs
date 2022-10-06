using Microsoft.AspNetCore.Mvc;
using Monitor.Grains;
using Monitor.Shared;
using Orleans;

namespace Monitor.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        private readonly IClusterClient _client;

        public MonitorController(IClusterClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDatabases()
        {
            var allDatabasesGrain = _client.GetGrain<IAllDatabasesGrain>(Constants.AllDatabasesGrainKey);
            var allDatabases = await allDatabasesGrain.GetAll();

            return Ok(allDatabases);
        }
    }
}
