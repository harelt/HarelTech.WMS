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
using Microsoft.Extensions.Caching.Memory;

namespace HarelTech.WMS.Repository
{
    public class Priority : IPriority
    {
        private readonly string _sqlConnectionString;
        private readonly DateTime _baseDateTime = new DateTime(1988, 1, 1, 0, 0, 0);
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

        public async Task<TasksSummerize> GetTaskSummerise(long userId, long warhouseId)
        {
            var ts = new TasksSummerize();
            using var db = DbConnection;
            //tasks by type
            var qry = new System.Text.StringBuilder($@"SELECT HWMS_ITASKTYPE as Task , COUNT(*) as Count
                FROM HWMS_ITASKS
                WHERE HWMS_ITASKSTATUS NOT IN ( 'F' , 'C' )
                AND HWMS_ITASKWARHS = {warhouseId}
                AND HWMS_ITASK > 0
                AND ( HWMS_ASSIGNUSER = 0  OR HWMS_ASSIGNUSER = {userId} )
                GROUP BY HWMS_ITASKTYPE;
            ");
            //open tasks
            qry.AppendLine($@"SELECT  COUNT(1) as OpenTasks
                FROM  HWMS_ITASKS 
                WHERE HWMS_ITASKSTATUS NOT IN ( 'F' , 'C' )
                AND   HWMS_ITASKWARHS = {warhouseId}
                AND   HWMS_ITASK > 0
                AND ( HWMS_ASSIGNUSER = 0  OR HWMS_ASSIGNUSER = {userId} ) ;
            ");
            var d = DateTime.Now.Date;
            var dnumber = d.Subtract(_baseDateTime).Minutes;
            qry.AppendLine($@"SELECT  COUNT(1) as TotalDone
                FROM  HWMS_ITASKS 
                WHERE HWMS_ITASKSTATUS = 'F'
                AND   HWMS_ITASKWARHS  = {warhouseId}
                AND   HWMS_ITASK > 0
                AND   HWMS_ITASKFDATE  = {dnumber}
                AND   HWMS_USERUPDATED = 5
                AND ( HWMS_ASSIGNUSER  = 0  OR HWMS_ASSIGNUSER = {userId} ) ;
            ");

            db.Open();
            var multi = await db.QueryMultipleAsync(qry.ToString(), null);
            
            ts.Tasks = multi.Read<TaskSum>().ToList();
            ts.Open  = multi.Read<int>().Single();
            ts.Total = multi.Read<int>().Single();
            ts.Total += ts.Open;
            
            return ts;
        }

        public async Task<List<TaskType>> GetTasktypes()
        {
            using(Context)
            {
                var results = await Context.Tasktypes.Where(w => w.HWMS_ITASKTYPE != 0).ToListAsync().ConfigureAwait(false);
                Parallel.ForEach(results, item =>
                {
                    char[] arr = item.HWMS_ITASKTYPEDES.ToCharArray();
                    Array.Reverse(arr);
                    item.HWMS_ITASKTYPEDES = new string(arr);
                });
                
                return results;
            }
        }

        private async Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroupOrder(long userId, 
            long warhouseId, EnumTaskType taskType)
        {
            var qry = @$"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE , HWMS_ITASKTYPES.HWMS_ITASKTYPENAME ,reverse(HWMS_REFTYPES.HWMS_REFTYPENAME) as HWMS_REFTYPENAME ,HWMS_ITASKS.HWMS_REFORDER , reverse(HWMS_ITASKS.HWMS_REFNAME) as HWMS_REFNAME ,
            ( COUNT(*)-SUM(CASE WHEN HWMS_ITASKS.HWMS_ITASKSTATUS = 'F' then 0 else  1 end)) as CompleteTasks,
            COUNT(*) as TotalTasks
            FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES
            WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
            AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
            AND  HWMS_ITASKSTATUS NOT IN ( 'C' )
            AND  HWMS_ITASKS.HWMS_ITASKWARHS = {warhouseId}
            AND  HWMS_ITASKTYPES.HWMS_ITASKTYPE = {(int)taskType}
            AND  HWMS_ITASKS.HWMS_ITASK > 0
            AND ( HWMS_ITASKS.HWMS_ASSIGNUSER = 0  OR HWMS_ITASKS.HWMS_ASSIGNUSER = {userId} )
            AND  HWMS_ITASKS.HWMS_REFORDERFIN <> 'Y'
            GROUP BY HWMS_ITASKTYPES.HWMS_ITASKTYPE , HWMS_ITASKTYPES.HWMS_ITASKTYPENAME , HWMS_REFTYPES.HWMS_REFTYPENAME ,HWMS_ITASKS.HWMS_REFORDER , HWMS_ITASKS.HWMS_REFNAME  
            ";
            using var db = DbConnection;
            db.Open();
            var results = await db.QueryAsync<CompleteTasksByGroup>(qry);

            return results.ToList();

        }

        private async Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroupZone(string qry)
        {
            using var db = DbConnection;
            db.Open();
            var results = await db.QueryAsync<CompleteTasksByGroup>(qry);

            return results.ToList();
        }


        public async Task<List<CompleteTasksByGroup>> GetCompleteTasksByGroup(long userId, long warhouseId, EnumTaskType enumTaskType, EnumTaskGroup enumTaskGroup)
        {
            if (enumTaskGroup == EnumTaskGroup.Order)
                return await GetCompleteTasksByGroupOrder(userId, warhouseId, enumTaskType).ConfigureAwait(false);
            var qry = "";
            if (new[] { EnumTaskType.Pick, EnumTaskType.Ship, EnumTaskType.Transfer}.Contains(enumTaskType))
            {
                qry = @$"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE, reverse(HWMS_WZONES.HWMS_WZONENAME) as HWMS_REFNAME, HWMS_WZONES.HWMS_WZONECODE as HWMS_REFORDER , reverse(HWMS_WZONES.HWMS_WZONENAME) as HWMS_WZONENAME  , 'Zone' as HWMS_REFTYPENAME,
                ( COUNT(*)-SUM(CASE WHEN HWMS_ITASKS.HWMS_ITASKSTATUS = 'F' then 0 else  1 end)) as CompleteTasks,
                COUNT(*) as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_WZONES 
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_ITASKS.HWMS_ITASKFZONE =  HWMS_WZONES.HWMS_WZONE
                AND  HWMS_ITASKSTATUS NOT IN (  'C' )
                AND  HWMS_ITASKS.HWMS_ITASKWARHS = {warhouseId}
                AND  HWMS_ITASKTYPES.HWMS_ITASKTYPE = {(int)enumTaskType}
                AND  HWMS_ITASKS.HWMS_ITASK > 0
                AND ( HWMS_ITASKS.HWMS_ASSIGNUSER = 0  OR HWMS_ITASKS.HWMS_ASSIGNUSER = {userId} )
                AND  HWMS_ITASKS.HWMS_REFORDERFIN <> 'Y'
                GROUP BY HWMS_ITASKTYPES.HWMS_ITASKTYPE, HWMS_ITASKTYPES.HWMS_ITASKTYPENAME, HWMS_WZONES.HWMS_WZONECODE, HWMS_WZONES.HWMS_WZONENAME 
                ";
                return await GetCompleteTasksByGroupZone(qry).ConfigureAwait(false);
            }

            qry = @$"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE, reverse(HWMS_WZONES.HWMS_WZONENAME) as HWMS_REFNAME, HWMS_WZONES.HWMS_WZONECODE , reverse(HWMS_WZONES.HWMS_WZONENAME) as HWMS_WZONENAME  , 'Zone' as HWMS_REFTYPENAME,
                ( COUNT(*)-SUM(CASE WHEN HWMS_ITASKS.HWMS_ITASKSTATUS = 'F' then 0 else  1 end)) as CompleteTasks,
                COUNT(*) as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_WZONES 
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_ITASKS.HWMS_ITASKTZONE =  HWMS_WZONES.HWMS_WZONE
                AND  HWMS_ITASKSTATUS NOT IN (  'C' )
                AND  HWMS_ITASKS.HWMS_ITASKWARHS = {warhouseId}
                AND  HWMS_ITASKTYPES.HWMS_ITASKTYPE = {(int)enumTaskType}
                AND  HWMS_ITASKS.HWMS_ITASK > 0
                AND ( HWMS_ITASKS.HWMS_ASSIGNUSER = 0  OR HWMS_ITASKS.HWMS_ASSIGNUSER = {userId} )
                AND  HWMS_ITASKS.HWMS_REFORDERFIN <> 'Y'
                GROUP BY HWMS_ITASKTYPES.HWMS_ITASKTYPE, HWMS_ITASKTYPES.HWMS_ITASKTYPENAME, HWMS_WZONES.HWMS_WZONECODE, HWMS_WZONES.HWMS_WZONENAME 
                ";
            return await GetCompleteTasksByGroupZone(qry).ConfigureAwait(false);

        }

        /// <summary>
        /// Show all Tasks related to Group, by  order/doc #
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="warhouseId"></param>
        /// <param name="enumTaskType"></param>
        /// <param name="refOrder"></param>
        /// <returns></returns>
        private async Task<List<CompleteTaskItem>> GetCompleteTaskItemsByGroupOrder(long userId, long warhouseId, EnumTaskType enumTaskType, string refOrder)
        {
            var qry = $@"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE, reverse(HWMS_ITASKTYPES.HWMS_ITASKTYPEDES) as  HWMS_ITASKTYPEDES, 
            HWMS_ITASKS.HWMS_REFORDER  ,  
            HWMS_ITASKS.HWMS_ITASKTZONE, HWMS_ITASKS.HWMS_ITASKFZONE,
            HWMS_ITASKS.HWMS_ITASK , PART.PARTNAME , reverse(PART.PARTDES) as PARTDES,
            HWMS_ITASKS.HWMS_ITASKFROMBIN , HWMS_ITASKS.HWMS_ITASKTOBIN , 
            HWMS_ITASKS.HWMS_COMPLETEDQTY/1000 as CompletedTasks, HWMS_ITASKS.HWMS_TOTALQTY/1000 as TotalTasks
            FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES , PART
            WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
            AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
            AND  HWMS_ITASKS.HWMS_ITASKPART = PART.PART
            AND  HWMS_ITASKSTATUS NOT IN ( 'C' )
            AND  HWMS_ITASKS.HWMS_ITASKWARHS = {warhouseId}
            AND  HWMS_ITASKTYPES.HWMS_ITASKTYPE = {(int)enumTaskType}
            AND  HWMS_ITASKS.HWMS_ITASK > 0
            AND ( HWMS_ITASKS.HWMS_ASSIGNUSER = 0  OR HWMS_ITASKS.HWMS_ASSIGNUSER = {userId} )
            AND  HWMS_ITASKS.HWMS_REFORDERFIN <> 'Y'  
            AND  HWMS_ITASKS.HWMS_REFORDER = '{refOrder}'
            ";
            using var db = DbConnection;
            db.Open();
            var results = await db.QueryAsync<CompleteTaskItem>(qry);

            return results.ToList();
        }
        private async Task<List<CompleteTaskItem>> GetCompleteTaskItemsByGroupZone(string qry)
        {
            using var db = DbConnection;
            db.Open();
            var results = await db.QueryAsync<CompleteTaskItem>(qry);

            return results.ToList();
        }
        public async Task<List<CompleteTaskItem>> GetCompleteTaskItemsByGroup(long userId, long warhouseId, EnumTaskType enumTaskType, EnumTaskGroup enumTaskGroup, string refOrderOrZone)
        {
            if (enumTaskGroup == EnumTaskGroup.Order)
                return await GetCompleteTaskItemsByGroupOrder(userId, warhouseId, enumTaskType, refOrderOrZone);

            var qry = "";
            if (new[] { EnumTaskType.Pick, EnumTaskType.Ship, EnumTaskType.Transfer }.Contains(enumTaskType))
            {
                qry = $@"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE, reverse(HWMS_ITASKTYPES.HWMS_ITASKTYPEDES) as  HWMS_ITASKTYPEDES, 
                HWMS_ITASKS.HWMS_REFORDER,  
                HWMS_ITASKS.HWMS_ITASKTZONE, HWMS_ITASKS.HWMS_ITASKFZONE,
                HWMS_ITASKS.HWMS_ITASK , PART.PARTNAME, reverse(PART.PARTDES) as PARTDES,
                HWMS_ITASKS.HWMS_ITASKFROMBIN,  
                HWMS_ITASKS.HWMS_COMPLETEDQTY/1000 as CompletedTasks, HWMS_ITASKS.HWMS_TOTALQTY/1000 as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES , PART, HWMS_WZONES 
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
                AND  HWMS_ITASKS.HWMS_ITASKPART = PART.PART
                AND  HWMS_ITASKSTATUS NOT IN ( 'C' )
                AND  HWMS_ITASKS.HWMS_ITASKWARHS = {warhouseId}
                AND  HWMS_ITASKTYPES.HWMS_ITASKTYPE = {(int)enumTaskType}
                AND  HWMS_ITASKS.HWMS_ITASK > 0
                AND ( HWMS_ITASKS.HWMS_ASSIGNUSER = 0  OR HWMS_ITASKS.HWMS_ASSIGNUSER = {userId} )
                AND  HWMS_ITASKS.HWMS_REFORDERFIN <> 'Y'  
                AND  HWMS_ITASKS.HWMS_ITASKFZONE =  HWMS_WZONES.HWMS_WZONE
                AND HWMS_WZONES.HWMS_WZONECODE = '{refOrderOrZone}'
                ";

                return await GetCompleteTaskItemsByGroupZone(qry);
            }
            else
            {
                qry = $@"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE, reverse(HWMS_ITASKTYPES.HWMS_ITASKTYPEDES) as  HWMS_ITASKTYPEDES, 
                HWMS_ITASKS.HWMS_REFORDER, 
                HWMS_ITASKS.HWMS_ITASKTZONE, HWMS_ITASKS.HWMS_ITASKFZONE,
                HWMS_ITASKS.HWMS_ITASK, PART.PARTNAME, reverse(PART.PARTDES) as PARTDES,
                HWMS_ITASKS.HWMS_ITASKTOBIN,  
                HWMS_ITASKS.HWMS_COMPLETEDQTY/1000 as CompletedTasks, HWMS_ITASKS.HWMS_TOTALQTY/1000 as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES , PART, HWMS_WZONES       
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
                AND  HWMS_ITASKS.HWMS_ITASKPART = PART.PART
                AND  HWMS_ITASKSTATUS NOT IN ( 'C' )
                AND  HWMS_ITASKS.HWMS_ITASKWARHS = {warhouseId}
                AND  HWMS_ITASKTYPES.HWMS_ITASKTYPE = {(int)enumTaskType}
                AND  HWMS_ITASKS.HWMS_ITASK > 0
                AND ( HWMS_ITASKS.HWMS_ASSIGNUSER = 0  OR HWMS_ITASKS.HWMS_ASSIGNUSER = {userId} )
                AND  HWMS_ITASKS.HWMS_REFORDERFIN <> 'Y'  
                AND  HWMS_ITASKS.HWMS_ITASKTZONE =  HWMS_WZONES.HWMS_WZONE
                AND HWMS_WZONES.HWMS_WZONECODE = '{refOrderOrZone}'"
                ;
                return await GetCompleteTaskItemsByGroupZone(qry);
            }
        }
    }
}
