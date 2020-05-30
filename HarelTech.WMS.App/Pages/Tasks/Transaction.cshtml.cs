using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HarelTech.WMS.App.Models;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace HarelTech.WMS.App.Pages.Tasks
{
    public class TransactionModel : PageModel
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
        public CompleteTaskItem TaskItem { get; set; }
        [BindProperty]
        public List<TaskLotSerial> TaskLot { get; set; }
        [BindProperty]
        public bool DisplayFromBin { get; set; }
        [BindProperty]
        public bool ChangeFromBin { get; set; }
        [BindProperty]
        public bool DisplayToBin { get; set; }
        [BindProperty]
        public bool ChangeToBin { get; set; }
        public string UserName;
        public string Password;
        public string TaskLotJson { get; set; }
        public TransactionModel(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }
        public async Task<IActionResult> OnGet(CompleteTaskItem taskItem)
        {
            var userid = Utilities.UserId(User.Claims);
            ViewData["PageTitle"] = "Transaction";
            Company = _cache.Get<string>($"{userid}_company");
            WarhouseId = _cache.Get<long>($"{userid}_warhouseId");
            CurrentTaskType = _cache.Get<EnumTaskType>($"{userid}_taskType");
            TaskItem = taskItem;
            UserName = Utilities.UserLogin(User.Claims);
            Password = Utilities.Password(User.Claims);

            TaskLot = await _wmsClient.GetTransactionItems(new TransactionItemsRequest
            {
                Company = Company,
                ParId = taskItem.PART,
                WarhouseId = WarhouseId
            });

            switch (CurrentTaskType)
            {
                case EnumTaskType.Pick:
                    DisplayFromBin = true; ChangeFromBin = false; DisplayToBin = true; ChangeToBin = true;
                    break;
                case EnumTaskType.Ship:
                    DisplayFromBin = true; ChangeFromBin = false; DisplayToBin = false; ChangeToBin = false;
                    break;
                case EnumTaskType.Receive:
                    DisplayFromBin = false; ChangeFromBin = false; DisplayToBin = true; ChangeToBin = true;
                    break;
                case EnumTaskType.Store:
                    DisplayFromBin = true; ChangeFromBin = false; DisplayToBin = true; ChangeToBin = true;
                    break;
                case EnumTaskType.Transfer:
                    DisplayFromBin = true; ChangeFromBin = false; DisplayToBin = true; ChangeToBin = true;
                    break;
                default:
                    break;
            }

            TaskLotJson = JsonSerializer.Serialize(TaskLot, new JsonSerializerOptions() { WriteIndented=false, AllowTrailingCommas =true });
            return Page();
        }

        public async Task<IActionResult> OnPostAddTaskLots([FromBody]List<TaskLotSerial> taskLots)
        {
            var userid = Utilities.UserId(User.Claims);
            var result = await _wmsClient.AddTaskLots(new AddTaskLotsRequest
            {
                Company = _cache.Get<string>($"{userid}_company"),
                Lots = taskLots
            });

            return new JsonResult(new { Success = result });
        }

        public async Task<IActionResult> OnGetBins()
        {
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            WarhouseId = _cache.Get<long>($"{userid}_warhouseId");
            var key = $"{WarhouseId}_bins";
            if (_cache.TryGetValue(key, out List<string> bins))
                return new JsonResult(new { Success = true, bins });

            bins = await _wmsClient.GetBins(Company, WarhouseId);
            if(bins != null && bins.Count > 0)
            {
                _cache.Set(key, bins, DateTime.Now.AddHours(2));
                return new JsonResult(new { Success = true, bins });
            }

            return new JsonResult(new { Success = false });

        }

        public async Task<IActionResult> OnDeleteTaskLots(long taskId)
        {
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            var result = await _wmsClient.DeleteTaskLots(Company, taskId);
            return new JsonResult(new { Success = true, result });
        }
    }
}