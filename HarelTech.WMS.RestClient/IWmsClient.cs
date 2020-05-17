﻿using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HarelTech.WMS.RestClient
{
    public interface IWmsClient
    {
        Task<SystemUser> GetSystemUserAsync(long userId);
        Task<List<UserWarhouse>> GetUserWarhousesAsync(string company, long userId);
        Task<TasksSummerize> GetTasksSummerizeAsync(long userId, long warhouseId, string company);
        Task<List<TaskType>> GetTaskTypesAsync(string company);
        Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroups(CompleteTasksByGroupRequest request);
        Task<List<CompleteTaskItem>> GetCompleteTaskItems(CompleteTaskItemsRequest request);
    }
}
