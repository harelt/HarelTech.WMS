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
using HarelTech.WMS.App.Models;

namespace HarelTech.WMS.App.Pages
{
    [Authorize]
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
        public string CompanyName { get; set; }
        public string WarhouseName { get; set; }
        public string UserName { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            Company = _cache.Get<string>($"{Utilities.UserId(User.Claims)}_company");
            if (string.IsNullOrEmpty(Company))
                return RedirectToPage("Account/Login");
            Warhouse = _cache.Get<long>($"{Utilities.UserId(User.Claims)}_warhouseId");
            CompanyName = _cache.Get<string>($"{Utilities.UserId(User.Claims)}_companyName");
            WarhouseName = _cache.Get<string>($"{Utilities.UserId(User.Claims)}_warhouseName");
            UserName = Utilities.UserName(User.Claims);

            TasksSummerize = await _wmsClient.GetTasksSummerizeAsync(Utilities.UserId(User.Claims), Warhouse, Company);
            TaskTypes = await _wmsClient.GetTaskTypesAsync(Company);
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
