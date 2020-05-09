using System.Threading.Tasks;

namespace HarelTech.WMS.Repository.Interfaces
{
    public interface IPriorityDbs
    {
        Priority Db(string company);
    }
}
