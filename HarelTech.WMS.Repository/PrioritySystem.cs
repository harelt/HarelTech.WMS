using HarelTech.WMS.Common.Entities;
using HarelTech.WMS.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace HarelTech.WMS.Repository
{
    public class PrioritySystem: IPrioritySystem
    {
        private readonly string _sqlConnectionString;
        private readonly DbContextOptionsBuilder<PrioritySystemContext> _priorityOptionBuilder;

        public PrioritySystem(string sqlConnectionString)
        {
            _priorityOptionBuilder = new DbContextOptionsBuilder<PrioritySystemContext>()
                .UseSqlServer(sqlConnectionString, providerOptions => providerOptions.CommandTimeout(60));
            _sqlConnectionString = sqlConnectionString;
        }

        private PrioritySystemContext SystemContext => new PrioritySystemContext(_priorityOptionBuilder.Options);
        private IDbConnection DbConnection => new SqlConnection(_sqlConnectionString);

        public async Task<int> GetCurrentActiveUsers()
        {
            using var db = DbConnection;
            var qry = $@"SELECT count(1) as Count from HWMS_USERS where HWMS_ACTIVE = 'Y'";
            var users = await db.QueryAsync<int>(qry);
            return users.First();
        }

        public async Task<SystemUser> GetSystemUser(string userLogin)
        {
            using var db = DbConnection;
            var qry = $@"SELECT [USERLOGIN],[T$USER] as Id, [USERNAME], [USERID], [USERGROUP] 
                FROM USERS where USERLOGIN = system.dbo.hebconvert('{userLogin}')";
            db.Open();
            
            var result = await db.QueryAsync<SystemUser>(qry);

            return result?.FirstOrDefault() ?? new SystemUser { USERLOGIN = userLogin };
            
        }

        public async Task<bool> IsUserExists(long userId)
        {
            using var db = DbConnection;
            var qry = $@"SELECT * from HWMS_USERS where HWMS_USER = {userId}";
            var user = await db.QueryAsync<WmsUser>(qry);
            if (user != null && user.FirstOrDefault().HWMS_ACTIVE == "Y") return true;
            return false;
            
        }
    }
}
