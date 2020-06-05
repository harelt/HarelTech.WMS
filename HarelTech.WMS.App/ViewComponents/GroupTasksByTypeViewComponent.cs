using HarelTech.WMS.Common.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarelTech.WMS.App.ViewComponents
{
    public class GroupTasksByTypeViewComponent: ViewComponent
    {
        private readonly IWmsClient _wmsClient;
        public GroupTasksByTypeViewComponent(IWmsClient wmsClient)
        {
            _wmsClient = wmsClient;
        }

        public async Task<IViewComponentResult> InvokeAsync(CompleteTasksByGroupRequest request)
        {
            
            var results = await _wmsClient.GetCompleteTasksByGroups(request).ConfigureAwait(false);
            results.RemoveAll(w => w.TotalTasks == w.CompleteTasks);
            ViewData["TaskType"] = request.TaskType;
            return View(results);
        }

    }
}
