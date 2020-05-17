using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HarelTech.WMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IPriorityDbs _priority;

        public TasksController(IPriorityDbs priority)
        {
            _priority = priority;
        }
       
        [HttpGet("Dashboard/{userId}/{warhouseId}/{company}")]
        public async Task<IActionResult> GetTasksSummerize(long userId, long warhouseId, string company )
        {
            if (userId == 0 || warhouseId == 0 || string.IsNullOrEmpty(company))
                return BadRequest("Invalid parameters request");

            var results =  await _priority.CompanyDb(company).GetTaskSummerise(userId, warhouseId);
            return Ok(results);
        }

        [HttpGet("TaskTypes/{company}")]
        public async Task<IActionResult> GetTaskTypesAsync(string company)
        {
            if (string.IsNullOrEmpty(company))
                return BadRequest("Company is required field");
            var results = await _priority.CompanyDb(company).GetTasktypes();
            return Ok(results);
        }

        [HttpPost("CompleteTasksByGroup")]
        public async Task<IActionResult> CompleteTasksByGroupAsync([FromBody]CompleteTasksByGroupRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var results = await _priority.CompanyDb(request.Company).GetCompleteTasksByGroup(request.UserId,
                request.WarhouseId, request.TaskType, request.TaskGroup);

            return Ok(results);
        }

        [HttpPost("CompleteTaskItems")]
        public async Task<IActionResult> CompleteTaskItems([FromBody]CompleteTaskItemsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var results = await _priority.CompanyDb(request.Company).GetCompleteTaskItemsByGroup(request.UserId,
                request.WarhouseId, request.TaskType, request.TaskGroup, request.RefOrderOrZone);

            return Ok(results);
        }

    }
}