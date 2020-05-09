using HarelTech.WMS.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HarelTech.WMS.Repository.Interfaces
{
    public interface IPrioritySystem
    {
        Task<int> GetCurrentActiveUsers();
        Task<bool> IsUserExists(long userId);
        Task<SystemUser> GetSystemUser(string userLogin);
    }
}
