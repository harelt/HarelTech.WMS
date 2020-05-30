﻿using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarelTech.WMS.Repository.Interfaces
{
    public interface IPriority
    {
        Task<List<Warhouse>> GetWarhouses();
        Task<DefaultWarhouseResponse> GetDefaultWarhouse(long user);
        Task<List<UserWarhouse>> GetUserWarhouses(long userId);
        Task<List<TaskType>> GetTasktypes();
        Task<TasksSummerize> GetTaskSummerise(long userId, long warhouseId);
        Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroup(long userId, long warhouseId, EnumTaskType taskType, EnumTaskGroup taskGroup);
        Task<List<CompleteTaskItem>> GetCompleteTaskItemsByGroup(long userId, long warhouseId, EnumTaskType enumTaskType, EnumTaskGroup enumTaskGroup, string refOrderOrZone);
        Task<List<TaskLotSerial>> GetTransactionLotSerial(long warhouseId, long partId);
        Task<bool> AddTaskLots(List<TaskLot> taskLots);
        Task<List<string>> GetBins(long warhouseId);

        Task<int> DeleteTaskLots(long taskId);
    }
}
