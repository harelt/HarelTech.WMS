using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Common.Entities;
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

            if (uwhrs == null || uwhrs.Count == 0) return Ok(new List<Warhouse>());

            var dw = uwhrs.FirstOrDefault(w => w.IsDefault == true).HWMS_WARHS;
            //reverse desc
            foreach (var item in whrs)
            {
                var arr = item.WARHSDES.ToCharArray();
                Array.Reverse(arr);
                item.WARHSDES = new string(arr);
                if (item.WARHS == dw)
                    item.IsDefault = true;
            }

            return Ok(whrs);
        }
    }
}