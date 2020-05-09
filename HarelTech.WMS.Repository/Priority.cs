using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using HarelTech.WMS.Common.Models;
using HarelTech.WMS.Repository.Interfaces;
using HarelTech.WMS.Common.Entities;

namespace HarelTech.WMS.Repository
{
    public class Priority : IPriority
    {
        private readonly string _sqlConnectionString;
        private readonly DateTime baseDateTime = new DateTime(1988, 1, 1, 0, 0, 0);
        private readonly DbContextOptionsBuilder<PriorityContext> _priorityOptionBuilder;

        public Priority(string sqlConnectionString)
        {
            _priorityOptionBuilder = new DbContextOptionsBuilder<PriorityContext>()
                .UseSqlServer(sqlConnectionString, providerOptions => providerOptions.CommandTimeout(60));
            _sqlConnectionString = sqlConnectionString;
        }

        private PriorityContext Context => new PriorityContext(_priorityOptionBuilder.Options);

        public IDbConnection DbConnection => new SqlConnection(_sqlConnectionString);

        public async Task<DefaultWarhouseResponse> GetDefaultWarhouse(long userId)
        {
            var qry = $@"SELECT WAREHOUSES.WARHSNAME , WAREHOUSES.WARHSDES 
                FROM USERSA, WAREHOUSES
                WHERE USERSA.WARHS = WAREHOUSES.WARHS
                AND USERSA.USER = {userId}
                AND    WAREHOUSES.WARHS > 0";
            qry = qry.Replace("\r\n", "");
            qry = qry.Replace("/t", "");
            using var conn = DbConnection;
            DbConnection.Open();

            var dw = await conn.QueryAsync<DefaultWarhouseResponse>(qry);
            return dw.FirstOrDefault();

        }

        public async Task<List<Warhouse>> GetWarhouses()
        {
            using (Context)
            {
                return await Context.Warhouses.ToListAsync();
            }
        }

        public async Task<List<UserWarhouse>> GetUserWarhouses(long userId)
        {
            using (Context)
            {
                return await Context.UsersWarhouses.Where(w => w.HWMS_USER == userId).ToListAsync();
            }
        }
    }
}
