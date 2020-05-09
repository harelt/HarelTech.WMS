using HarelTech.WMS.Common.Entities;
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
    }
}
