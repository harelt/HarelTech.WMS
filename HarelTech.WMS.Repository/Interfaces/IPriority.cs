using HarelTech.WMS.Common.Models;
using System.Threading.Tasks;

namespace HarelTech.WMS.Repository
{
    public interface IPriority
    {
        Task<DefaultWarhouseResponse> GetDefaultWarhouse(long user);
    }
}
