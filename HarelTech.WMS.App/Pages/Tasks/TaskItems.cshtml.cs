using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarelTech.WMS.App.Models;
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
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            WarhouseId = _cache.Get<long>($"{userid}_warhouseId");
            CurrentTaskType = taskType;
            var taskGroup = _cache.Get<EnumTaskGroup>($"{userid}_taskGroup");
            _cache.Set($"{userid}_taskType", taskType);
            //var company = Request.Cookies["company"] ?? "smoukr";

            var tasksTypes = await _wmsClient.GetTaskTypesAsync(Company);
            ViewData["PageTitle"] = tasksTypes.FirstOrDefault(w => w.HWMS_ITASKTYPE == (int)taskType).HWMS_ITASKTYPEDES + " Tasks";

            CompleteTaskItems = await _wmsClient.GetCompleteTaskItems(new CompleteTaskItemsRequest
            {
                Company = Company,
                RefOrderOrZone = refOrderZone,
                TaskType = CurrentTaskType,
                TaskGroup = taskGroup,
                UserId = userid,
                WarhouseId = WarhouseId
            });
            CompleteTaskItems.ForEach(f => f.RefOrderOrZone = refOrderZone);
            _cache.Set($"{userid}_CompleteTaskItems", CompleteTaskItems);
            return await Task.FromResult(Page());
        }

        public async Task<IActionResult> OnGetTaskTransaction(long part)
        {
            var userid = Utilities.UserId(User.Claims);
            CompleteTaskItems = _cache.Get<List<CompleteTaskItem>>($"{userid}_CompleteTaskItems");
            var taskItem = CompleteTaskItems.FirstOrDefault(w => w.PART == part);

            return await Task.FromResult(RedirectToPage("Transaction", taskItem));
        }
    }
}