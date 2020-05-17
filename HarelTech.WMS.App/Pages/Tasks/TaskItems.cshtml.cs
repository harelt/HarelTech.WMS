using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace HarelTech.WMS.App.Pages.Tasks
{
    public class TaskItemsModel : PageModel
    {
        private readonly IWmsClient _wmsClient;
        private readonly IMemoryCache _cache;

        [BindProperty]
        public string Company { get; set; }
        [BindProperty]
        public long WarhouseId { get; set; }
        [BindProperty]
        public EnumTaskType CurrentTaskType { get; set; }
        [BindProperty]
        public List<CompleteTaskItem> CompleteTaskItems { get; set; }
        public TaskItemsModel(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }
        public async Task<IActionResult> OnGet(EnumTaskType taskType, string refOrderZone)
        {
            Company = _cache.Get<string>("15_company");
            WarhouseId = _cache.Get<long>("15_warhouseId");
            CurrentTaskType = taskType;
            var taskGroup = _cache.Get<EnumTaskGroup>("15_taskGroup");
            //var company = Request.Cookies["company"] ?? "smoukr";

            var tasksTypes = await _wmsClient.GetTaskTypesAsync(Company);
            ViewData["PageTitle"] = tasksTypes.FirstOrDefault(w => w.HWMS_ITASKTYPE == (int)taskType).HWMS_ITASKTYPEDES + " Tasks";

            CompleteTaskItems = await _wmsClient.GetCompleteTaskItems(new CompleteTaskItemsRequest
            {
                Company = Company,
                RefOrderOrZone = refOrderZone,
                TaskType = CurrentTaskType,
                TaskGroup = taskGroup,
                UserId = 15,
                WarhouseId = WarhouseId
            });

            return await Task.FromResult(Page());
        }
    }
}