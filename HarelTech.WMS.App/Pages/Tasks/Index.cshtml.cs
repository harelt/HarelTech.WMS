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
    public class IndexModel : PageModel
    {
        private readonly IWmsClient _wmsClient;
        private readonly IMemoryCache _cache;

        public IndexModel(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }
        [BindProperty]
        public string Company { get; set; }
        [BindProperty]
        public long WarhouseId { get; set; }
        [BindProperty]
        public EnumTaskType CurrnetTaskType { get; set; }
        
        public async Task<IActionResult> OnGetAsync(EnumTaskType taskType)
        {
            Company = _cache.Get<string>("15_company");
            WarhouseId = _cache.Get<long>("15_warhouseId");
            CurrnetTaskType = taskType;
            
            var tasksTypes = await _wmsClient.GetTaskTypesAsync(Company);
            ViewData["PageTitle"] = tasksTypes.FirstOrDefault(w => w.HWMS_ITASKTYPE == (int)taskType).HWMS_ITASKTYPEDES + " Groups";
            


            return Page();
        }

        public async Task<IActionResult> OnGetGroupTasks(EnumTaskGroup taskGroup, EnumTaskType taskType)
        {
            var userId = 15;
            _cache.Set("15_taskGroup", taskGroup);
            return await Task.FromResult(ViewComponent("GroupTasksByType", new CompleteTasksByGroupRequest
            {
                Company = _cache.Get<string>("15_company"),
                TaskGroup = taskGroup,
                WarhouseId = _cache.Get<long>("15_warhouseId"),
                TaskType = taskType,
                UserId = userId
            }));
            
        }
               
    }
}