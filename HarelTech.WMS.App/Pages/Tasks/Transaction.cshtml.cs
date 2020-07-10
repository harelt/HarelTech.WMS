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
using Serilog;

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
        [BindProperty]
        public  List<ITaskLotModel> OpenedTaskLots { get; set; }
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
            Company = _cache.Get<string>($"{userid}_company");
            WarhouseId = _cache.Get<long>($"{userid}_warhouseId");
            CurrentTaskType = _cache.Get<EnumTaskType>($"{userid}_taskType");
            TaskItem = taskItem;
            UserName = Utilities.UserLogin(User.Claims);
            Password = Utilities.Password(User.Claims);

            var tasksTypes = await _wmsClient.GetTaskTypesAsync(Company);
            ViewData["PageTitle"] = tasksTypes.FirstOrDefault(w => w.HWMS_ITASKTYPE == (int)CurrentTaskType).HWMS_ITASKTYPEDES + " Transaction";
            OpenedTaskLots = new List<ITaskLotModel>();
            if (CurrentTaskType != EnumTaskType.Receive)
            {
                TaskLot = await _wmsClient.GetTransactionItems(new TransactionItemsRequest
                {
                    Company = Company,
                    ParId = taskItem.PART,
                    WarhouseId = WarhouseId
                });

                OpenedTaskLots = await _wmsClient.GetOpenedTaskLots(Company, taskItem.HWMS_ITASK);
            }
            else
                TaskLot = new List<TaskLotSerial>();

            if (TaskLot != null && TaskLot.Count >= 0)
            {
                await _wmsClient.ActivateTask(new ActivateTaskRequest
                {
                    Company = Company,
                    TaskId = taskItem.HWMS_ITASK,
                    UserId = userid
                });

                List<TaskLotSerial> selectedLot=null;
                if (CurrentTaskType != EnumTaskType.Receive)
                {
                    selectedLot = TaskLot.Where(w => w.FROMBIN == TaskItem.HWMS_ITASKFROMBIN).ToList();
                }
                else
                {
                    selectedLot = TaskLot.Where(w => w.TOBIN == TaskItem.HWMS_ITASKTOBIN).ToList();
                }
                if (selectedLot != null)
                {
                    var ids = selectedLot.Select(s => s.HWMS_ITASK).ToList();
                    TaskLot.RemoveAll(w =>  ids.Contains(w.HWMS_ITASK));
                    TaskLot.InsertRange(0, selectedLot);
                }
            }
            

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
            try
            {
                var userid = Utilities.UserId(User.Claims);
                if (taskLots.Any() && taskLots[0].Serials == null)
                {
                    var result = await _wmsClient.AddTaskLots(new AddTaskLotsRequest
                    {
                        Company = _cache.Get<string>($"{userid}_company"),
                        Lots = taskLots
                    });
                    return new JsonResult(new { Success = result });
                }

                //serials
                var taskLotId = await _wmsClient.AddTaskLot(new AddTaskLotsRequest
                {
                    Company = _cache.Get<string>($"{userid}_company"),
                    Lots = taskLots
                });

                return new JsonResult(new { taskLotId });

            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "OnPostAddTaskLots");
                return new JsonResult(new { Success = false });
            }
            

            
        }

        public async Task<IActionResult> OnGetBins()
        {
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            WarhouseId = _cache.Get<long>($"{userid}_warhouseId");

            return await Task.FromResult( ViewComponent("Bins", new { company = Company, warhouseId = WarhouseId }));

        }

        public async Task<IActionResult> OnGetSerials(long partId, long lot, long iTaskLot ,string locName="")
        {
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            return await Task.FromResult(ViewComponent("Serials", new { company = Company, partId, lot, locName, iTaskLot }));
        }

        public async Task<IActionResult> OnDeleteTaskLots(long taskId)
        {
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            var result = await _wmsClient.DeleteTaskLots(Company, taskId);
            return new JsonResult(new { Success = true, result });
        }

        public async Task<IActionResult> OnGetAddNewSerialCard(int id, string serialNumber)
        {
            ViewData["Id"] = id;
            ViewData["SerialNumber"] = serialNumber;

            return await Task.FromResult(new PartialViewResult
            {
                ViewName = "_newSerialCard",
                ViewData = ViewData
            });
        }

        public async Task<IActionResult> OnPostAddNewLot([FromBody]AddNewLotRequest request)
        {
            request.LotNumber = request.LotNumber == "" ? "0" : request.LotNumber;
            return await Task.FromResult(Partial("_newLotCard", request));
        }

        public async Task<IActionResult> OnGetDeleteOpenTaskSerials(long taskLot)
        {
            var userid = Utilities.UserId(User.Claims);
            Company = _cache.Get<string>($"{userid}_company");
            var result = await _wmsClient.DeleteOpenedTaskLotSerials(Company, taskLot);
                return new JsonResult(new { Success = result });
        }
    }
}