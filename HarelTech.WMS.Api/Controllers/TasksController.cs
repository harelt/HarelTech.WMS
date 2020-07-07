using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;
using ServiceStack.Text;

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
        public async Task<IActionResult> GetTasksSummerize(long userId, long warhouseId, string company)
        {
            if (userId == 0 || warhouseId == 0 || string.IsNullOrEmpty(company))
                return BadRequest("Invalid parameters request");

            var results = await _priority.CompanyDb(company).GetTaskSummerise(userId, warhouseId);
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

        [HttpPost("TransactionItems")]
        public async Task<IActionResult> GetTransactionItems(TransactionItemsRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var results = await _priority.CompanyDb(request.Company).GetTransactionLotSerial(request.WarhouseId,
                request.ParId);

            return Ok(results);
        }

        [HttpPost("AddTaskLots")]
        public async Task<IActionResult> AddTaskLots(AddTaskLotsRequest request)
        {
            Log.Information($"AddTaskLots: {request.Dump()}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lots = new List<TaskLot>();
            foreach (var item in request.Lots)
            {
                var tl = new TaskLot
                {
                    HWMS_EXPDATE = 0,
                    HWMS_FROMBIN = item.FROMBIN,
                    HWMS_ITASK = item.HWMS_ITASK,
                    HWMS_LOT = item.HWMS_LOT,
                    HWMS_LOTNUMBER = item.HWMS_ELOTNUMBER,
                    HWMS_LOTQUANTITY = item.Quantity * 1000,
                    HWMS_TOBIN = item.TOBIN,
                    HWMS_FCUSTNAME = item.HWMS_FCUSTNAME,
                    HWMS_TCUSTNAME = item.HWMS_TCUSTNAME,
                    Serials = item.Serials
                };
                if (item.HWMS_ELOTNUMBER == "0")
                    tl.HWMS_EXPDATE = 0;
                else
                {
                    CultureInfo uk = new CultureInfo("en-UK", false);
                    var expDate = DateTime.ParseExact(item.ExpDate, "dd/MM/yyyy", uk);
                    tl.HWMS_EXPDATE = (long)expDate.Subtract(new DateTime(1988, 1, 1, 0, 0, 0)).TotalMinutes;
                }

                lots.Add(tl);
            }
            
            var result = await _priority.CompanyDb(request.Company).AddTaskLots(lots);
            return Ok(result);
        }

        [HttpPost("AddTaskLot")]
        public async Task<IActionResult> AddTaskLot(AddTaskLotsRequest request)
        {
            Log.Information($"AddTaskLots: {request.Dump()}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lots = new List<TaskLot>();
            foreach (var item in request.Lots)
            {
                var tl = new TaskLot
                {
                    HWMS_ITASKLOT = item.HWMS_ITASKLOT,
                    HWMS_EXPDATE = 0,
                    HWMS_FROMBIN = item.FROMBIN,
                    HWMS_ITASK = item.HWMS_ITASK,
                    HWMS_LOT = item.HWMS_LOT,
                    HWMS_LOTNUMBER = item.HWMS_ELOTNUMBER,
                    HWMS_LOTQUANTITY = item.Quantity * 1000,
                    HWMS_TOBIN = item.TOBIN,
                    HWMS_FCUSTNAME = item.HWMS_FCUSTNAME,
                    HWMS_TCUSTNAME = item.HWMS_TCUSTNAME,
                    Serials = item.Serials
                };
                if (item.HWMS_ELOTNUMBER == "0")
                    tl.HWMS_EXPDATE = 0;
                else
                {
                    CultureInfo uk = new CultureInfo("en-UK", false);
                    var expDate = DateTime.ParseExact(item.ExpDate, "dd/MM/yyyy", uk);
                    tl.HWMS_EXPDATE = (long)expDate.Subtract(new DateTime(1988, 1, 1, 0, 0, 0)).TotalMinutes;
                }

                lots.Add(tl);
            }

            var addLot = await _priority.CompanyDb(request.Company).AddTaskLotSerials(lots[0]);
            return Ok(addLot);
        }

        [HttpGet("Bins/{company}/{warhouseId}")]
        public async Task<IActionResult> GetBins(string company, long warhouseId)
        {
            var results = await _priority.CompanyDb(company).GetBins(warhouseId);
            return Ok(results);
        }

        [HttpDelete("DeleteTaskLots/{company}/{taskId}")]
        public async Task<IActionResult> DeleteTaskLots(string company, long taskId)
        {
            var result = await _priority.CompanyDb(company).DeleteTaskLots(taskId);
            return Ok(result);
        }

        [HttpPost("ActivateTask")]
        public async Task<IActionResult> ActivateTask(ActivateTaskRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = await _priority.CompanyDb(request.Company).ActivateTask(request.TaskId, request.UserId);

            return Ok(result);
        }

        [HttpPost("Serials")]
        public async Task<IActionResult> GetSerials(SerialsRequest serialsRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(string.IsNullOrEmpty(serialsRequest.LocName))
                serialsRequest.LocName = "";
            var results = await _priority.CompanyDb(serialsRequest.Company).GetSerials(serialsRequest.PartId,
                serialsRequest.SerialId, serialsRequest.LocName);

            return Ok(results);
        }

        [HttpGet("Serials/Selected/{company}/{iTaskLot}")]
        public async Task<IActionResult> GetSelectedSerials(string company, long iTaskLot)
        {
            if (string.IsNullOrEmpty(company) || iTaskLot == 0)
                return BadRequest();
            var results = await _priority.CompanyDb(company).GetSelectedSerials(iTaskLot);
            return Ok(results);
        }

        [HttpGet("OpenedTaskLots/{company}/{taskId}")]
        public async Task<IActionResult> GetOpenedTaskLots(string company, long taskId)
        {
            if (string.IsNullOrEmpty(company) || taskId == 0)
                return BadRequest();

            var results = await _priority.CompanyDb(company).GetOpenedTaskLots(taskId);
            return Ok(results);
        }

        [HttpDelete("DeleteOpenTaskSerials/{company}/{taskLot}")]
        public async Task<IActionResult> DeleteOpenTaskSerialselete(string company, long taskLot)
        {
            if (string.IsNullOrEmpty(company) || taskLot == 0)
                return BadRequest();

            var results = await _priority.CompanyDb(company).DeleteOpenTaskSerials(taskLot);
            return Ok(results);
        }
    }
}