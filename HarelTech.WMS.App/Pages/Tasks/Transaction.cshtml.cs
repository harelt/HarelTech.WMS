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
        public TransactionModel(IWmsClient wmsClient, IMemoryCache cache)
        {
            _wmsClient = wmsClient;
            _cache = cache;
        }
        public async Task<IActionResult> OnGet(CompleteTaskItem taskItem)
        {
            ViewData["PageTitle"] = "Transaction";
            Company = _cache.Get<string>("15_company");
            WarhouseId = _cache.Get<long>("15_warhouseId");
            CurrentTaskType = _cache.Get<EnumTaskType>("15_taskType");
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
            if(CurrentTaskType == EnumTaskType.Pick)
            {
                
            }
            else if(CurrentTaskType == EnumTaskType.Ship)
            {
                
            }
            return Page();
        }
    }
}