using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Core;
using HarelTech.WMS.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace HarelTech.WMS.App.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly IWmsClient _wmsClient;
        private readonly IMemoryCache _cache;
        [BindProperty]
        public string Company { get; set; }
        [BindProperty]
        public long Warhouse { get; set; }
        [BindProperty]
        public List<TaskType> TaskTypes { get; set; }
        [BindProperty]
        public TasksSummerize TasksSummerize { get; set; }

        public IndexModel(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Company = _cache.Get<string>("15_company");
            Warhouse = _cache.Get<long>("15_warhouseId");
            TasksSummerize = await _wmsClient.GetTasksSummerizeAsync(5, -2, "smoukr");
            TaskTypes = await _wmsClient.GetTaskTypesAsync("smoukr");
            var ids = TasksSummerize.Tasks.Select(s => s.Task).ToList();
            var missingTypes = TaskTypes.Where(w => !ids.Contains(w.HWMS_ITASKTYPE));

            foreach (var item in missingTypes)
            {
                TasksSummerize.Tasks.Add(new TaskSum { Task = item.HWMS_ITASKTYPE, Count = 0 });
            }


            return Page();
        }
    }
}
