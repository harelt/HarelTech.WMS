using System;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HarelTech.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class WarhousesController : ControllerBase
    {
        private readonly IPriorityDbs _priority;
        public WarhousesController(IPriorityDbs priority)
        {
            _priority = priority;
        }


        [HttpGet("{company}/{userId}")]
        public async Task<IActionResult> GetAsync(string company, long userId)
        {
            var taskWhrs = _priority.CompanyDb(company).GetWarhouses();
            var taskUwhrs = _priority.CompanyDb(company).GetUserWarhouses(userId);
            await Task.WhenAll(taskWhrs, taskUwhrs);
            
            var whrs = taskWhrs.Result;
            var uwhrs = taskUwhrs.Result;

            var ids = uwhrs.Select(s => s.HWMS_WARHS);

            whrs.RemoveAll(w => !ids.Contains(w.WARHS));

            return Ok(whrs);
        }
    }
}