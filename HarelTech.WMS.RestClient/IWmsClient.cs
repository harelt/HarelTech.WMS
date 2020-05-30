using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HarelTech.WMS.RestClient
{
    public interface IWmsClient
    {
        Task<SystemUser> GetSystemUserAsync(UserLoginModel userLogin);
        Task<List<Warhouse>> GetUserWarhousesAsync(string company, long userId);
        Task<TasksSummerize> GetTasksSummerizeAsync(long userId, long warhouseId, string company);
        Task<List<TaskType>> GetTaskTypesAsync(string company);
        Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroups(CompleteTasksByGroupRequest request);
        Task<List<CompleteTaskItem>> GetCompleteTaskItems(CompleteTaskItemsRequest request);
        Task<List<TaskLotSerial>> GetTransactionItems(TransactionItemsRequest request);
        Task<bool> AddTaskLots(AddTaskLotsRequest request);
        Task<List<string>> GetBins(string company, long warhouseId);
        Task<int> DeleteTaskLots(string company, long taskId);
    }
}
