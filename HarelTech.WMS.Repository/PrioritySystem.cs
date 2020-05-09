using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HarelTech.WMS.Repository
{
    public class PrioritySystem: IPrioritySystem
    {
        //private readonly string _sqlConnectionString;
        private readonly DbContextOptionsBuilder<PrioritySystemContext> _priorityOptionBuilder;

        public PrioritySystem(string sqlConnectionString)
        {
            _priorityOptionBuilder = new DbContextOptionsBuilder<PrioritySystemContext>()
                .UseSqlServer(sqlConnectionString, providerOptions => providerOptions.CommandTimeout(60));
            //_sqlConnectionString = sqlConnectionString;
        }

        private PrioritySystemContext Context => new PrioritySystemContext(_priorityOptionBuilder.Options);

        public async Task<int> GetCurrentActiveUsers()
        {
            using (Context)
            {
                var users = await Context.WmsUsers.CountAsync(w => w.HWMS_ACTIVE == "Y").ConfigureAwait(false);
                return users;
            }
        }

        public async Task<SystemUser> GetSystemUser(string userLogin)
        {
            using (Context)
            {
                return await Context.SystemUsers.FirstOrDefaultAsync(w => w.USERLOGIN == userLogin);
            }
        }

        public async Task<bool> IsUserExists(long userId)
        {
            using (Context)
            {
                var user = await Context.WmsUsers.FindAsync(userId).ConfigureAwait(false);
                if (user != null && user.HWMS_ACTIVE == "Y") return true;
                return false;
            }
        }
    }
}
