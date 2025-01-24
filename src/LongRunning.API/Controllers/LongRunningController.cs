using Microsoft.AspNetCore.Mvc;

namespace LongRunning.API.Controllers
{
    [ApiController]
    [Route("long-running")]
    public class LongRunningController : ControllerBase
    {
        [HttpGet()]
        public async Task<OkResult> Get(int timeoutInSeconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds));
            return Ok();
        }
    }
}
