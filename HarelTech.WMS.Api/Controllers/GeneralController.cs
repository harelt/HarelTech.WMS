using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HarelTech.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            return await Task.FromResult(Ok(DateTime.Now.ToString()));
        }
    }
}