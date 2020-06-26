using System;
using System.Collections.Generic;
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
                var wrhs  = await Context.UsersWarhouses.Where(w => w.HWMS_USER == userId).ToListAsync();
                foreach (var item in wrhs)
                {
                    if(item.HWMS_WARHSDEF == "Y")
                        item.IsDefault = true;
                }
                return wrhs;
            }
        }

        public async Task<TasksSummerize> GetTaskSummerise(long userId, long warhouseId)
        {
            var ts = new TasksSummerize();
            using var db = DbConnection;
            //tasks by type
            var qry = new System.Text.StringBuilder($@"SELECT HWMS_ITASKTYPE as Task , COUNT(*) as Count
                FROM HWMS_ITASKS
                WHERE HWMS_ITASKSTATUS NOT IN ( 'F' , 'C', 'N' )
                AND HWMS_ITASKWARHS = {warhouseId}
                AND HWMS_ITASK > 0
                AND ( HWMS_ASSIGNUSER = 0  OR HWMS_ASSIGNUSER = {userId} )
                GROUP BY HWMS_ITASKTYPE;
            ");
            //open tasks
            qry.AppendLine($@"SELECT  COUNT(1) as OpenTasks
                FROM  HWMS_ITASKS 
                WHERE HWMS_ITASKSTATUS NOT IN ( 'F' , 'C', 'N' )
                AND   HWMS_ITASKWARHS = {warhouseId}
                AND   HWMS_ITASK > 0
                AND ( HWMS_ASSIGNUSER = 0  OR HWMS_ASSIGNUSER = {userId} ) ;
            ");
            var d = DateTime.Now.Date;
            var dnumber = d.Subtract(_baseDateTime).TotalMinutes;
            qry.AppendLine($@"SELECT  COUNT(1) as TotalDone
                FROM  HWMS_ITASKS 
                WHERE HWMS_ITASKSTATUS = 'F'
                AND   HWMS_ITASKWARHS  = {warhouseId}
                AND   HWMS_ITASK > 0
                AND   HWMS_ITASKFDATE  = {dnumber}
                AND   HWMS_USERUPDATED = {userId}
                AND ( HWMS_ASSIGNUSER  = 0  OR HWMS_ASSIGNUSER = {userId} ) ;
            ");

            db.Open();
            var multi = await db.QueryMultipleAsync(qry.ToString(), null);

            ts.Tasks = multi.Read<TaskSum>().ToList();
            ts.Total = multi.Read<int>().Single();
            ts.Done = multi.Read<int>().Single();
            ts.Total += ts.Done;

            return ts;
        }

        public async Task<List<TaskType>> GetTasktypes()
        {
            using (Context)
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
            AND  HWMS_ITASKSTATUS NOT IN (  'C', 'F', 'N' )
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
            if (new[] { EnumTaskType.Pick, EnumTaskType.Ship, EnumTaskType.Transfer }.Contains(enumTaskType))
            {
                qry = @$"SELECT HWMS_ITASKTYPES.HWMS_ITASKTYPE, reverse(HWMS_WZONES.HWMS_WZONENAME) as HWMS_REFNAME, HWMS_WZONES.HWMS_WZONECODE as HWMS_REFORDER , reverse(HWMS_WZONES.HWMS_WZONENAME) as HWMS_WZONENAME  , 'Zone' as HWMS_REFTYPENAME,
                ( COUNT(*)-SUM(CASE WHEN HWMS_ITASKS.HWMS_ITASKSTATUS = 'F' then 0 else  1 end)) as CompleteTasks,
                COUNT(*) as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_WZONES 
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_ITASKS.HWMS_ITASKFZONE =  HWMS_WZONES.HWMS_WZONE
                AND  HWMS_ITASKSTATUS NOT IN (  'C', 'F', 'N' )
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
                AND  HWMS_ITASKSTATUS NOT IN (  'C', 'F', 'N' )
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
            HWMS_ITASKS.HWMS_REFORDER, reverse(HWMS_ITASKS.HWMS_REFNAME) as HWMS_REFNAME, HWMS_ITASKNUM, 
            HWMS_ITASKS.HWMS_ITASKTZONE, HWMS_ITASKS.HWMS_ITASKFZONE, reverse(HWMS_REFTYPES.HWMS_REFTYPENAME) as HWMS_REFTYPENAME,  
            HWMS_ITASKS.HWMS_ITASK , PART.PARTNAME , reverse(PART.PARTDES) as PARTDES, PART.SERNFLAG, PART.PART, 
            HWMS_ITASKS.HWMS_ITASKFROMBIN , HWMS_ITASKS.HWMS_ITASKTOBIN , 
            HWMS_ITASKS.HWMS_COMPLETEDQTY/1000 as CompletedTasks, HWMS_ITASKS.HWMS_TOTALQTY/1000 as TotalTasks
            FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES , PART
            WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
            AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
            AND  HWMS_ITASKS.HWMS_ITASKPART = PART.PART
            AND  HWMS_ITASKSTATUS IN ( 'A', 'P' )
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
                HWMS_ITASKS.HWMS_REFORDER, reverse(HWMS_ITASKS.HWMS_REFNAME) as HWMS_REFNAME, HWMS_ITASKNUM,  
                HWMS_ITASKS.HWMS_ITASKTZONE, HWMS_ITASKS.HWMS_ITASKFZONE,
                HWMS_ITASKS.HWMS_ITASK , PART.PARTNAME, reverse(PART.PARTDES) as PARTDES, PART.SERNFLAG, PART.PART, 
                HWMS_ITASKS.HWMS_ITASKFROMBIN, reverse(HWMS_REFTYPES.HWMS_REFTYPENAME) as HWMS_REFTYPENAME, 
                HWMS_ITASKS.HWMS_COMPLETEDQTY/1000 as CompletedTasks, HWMS_ITASKS.HWMS_TOTALQTY/1000 as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES , PART, HWMS_WZONES 
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
                AND  HWMS_ITASKS.HWMS_ITASKPART = PART.PART
                AND  HWMS_ITASKSTATUS IN ( 'A', 'P' )
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
                HWMS_ITASKS.HWMS_REFORDER, reverse(HWMS_ITASKS.HWMS_REFNAME) as HWMS_REFNAME, HWMS_ITASKNUM,  
                HWMS_ITASKS.HWMS_ITASKTZONE, HWMS_ITASKS.HWMS_ITASKFZONE,
                HWMS_ITASKS.HWMS_ITASK, PART.PARTNAME, reverse(PART.PARTDES) as PARTDES, PART.SERNFLAG, PART.PART, 
                HWMS_ITASKS.HWMS_ITASKTOBIN, reverse(HWMS_REFTYPES.HWMS_REFTYPENAME) as HWMS_REFTYPENAME, 
                HWMS_ITASKS.HWMS_COMPLETEDQTY/1000 as CompletedTasks, HWMS_ITASKS.HWMS_TOTALQTY/1000 as TotalTasks
                FROM HWMS_ITASKS , HWMS_ITASKTYPES , HWMS_REFTYPES , PART, HWMS_WZONES       
                WHERE HWMS_ITASKS.HWMS_ITASKTYPE =  HWMS_ITASKTYPES.HWMS_ITASKTYPE
                AND  HWMS_REFTYPES.HWMS_REFTYPE  = HWMS_ITASKS.HWMS_REFTYPE
                AND  HWMS_ITASKS.HWMS_ITASKPART = PART.PART
                AND  HWMS_ITASKSTATUS IN ( 'A', 'P' )
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

        public async Task<List<TaskLotSerial>> GetTransactionLotSerial(long warhouseId, long partId)
        {
            var sql = $"select TRIM(WARHSNAME) from WAREHOUSES where WARHS = {warhouseId}";

            using var db = DbConnection;
            db.Open();
            var warhsname = await db.QuerySingleAsync<string>(sql).ConfigureAwait(false);

            var qry = $@"SELECT SERIAL.SERIAL as HWMS_LOT,  SERIAL.SERIALNAME as HWMS_ELOTNUMBER , CUSTOMERS.CUSTNAME AS STATUS , SERIAL.EXPIRYDATE ,WARHSBAL.BALANCE /1000 AS AVAILABLE,
            WAREHOUSES.LOCNAME AS FROMBIN , reverse(UNIT.UNITNAME) as  UNITNAME 
            FROM    WARHSBAL , WAREHOUSES , SERIAL , CUSTOMERS , PART ,UNIT
            WHERE   WAREHOUSES.WARHS      =    WARHSBAL.WARHS 
            AND     WARHSBAL.SERIAL       =    SERIAL.SERIAL
            AND     WARHSBAL.BALANCE  >  0  
            AND     CUSTOMERS.CUST        =    WARHSBAL.CUST
            AND     WARHSBAL.PART         =    PART.PART
            AND     PART.PART             =    {partId}
            AND     PART.UNIT             =    UNIT.UNIT 
            AND     WARHSBAL.ACT          =    0
            AND     WARHSBAL.CUST         =    CUSTOMERS.CUST 
            AND     WAREHOUSES.WARHSNAME  =   '{warhsname}' 
            AND     WAREHOUSES.LOCNAME    <>  '0'
            ORDER BY SERIAL.EXPIRYDATE , FROMBIN";

            var results = await db.QueryAsync<TaskLotSerial>(qry);
            foreach (var result in results)
            {
                result.ExpDate = _baseDateTime.AddMinutes(result.EXPIRYDATE).ToString("dd/MM/yyyy");
            }
            return results.ToList(); ;
        }

        public async Task<bool> AddTaskLots(List<TaskLot> taskLots)
        {
            using var ctx = Context;
            foreach (var lot in taskLots)
            {
                await ctx.TaskLots.AddAsync(lot).ConfigureAwait(false);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                if(lot.Serials != null && lot.Serials.Count > 0)
                {
                    var taskSerials = new List<TaskSerial>();
                    foreach (var item in lot.Serials)
                    {
                        taskSerials.Add(new TaskSerial
                        {
                            HWMS_SERN = item.SerialId,
                            HWMS_SERNUMBER = item.SerialNumber,
                            HWMS_ITASKLOT = lot.HWMS_ITASKLOT
                        });
                    }
                    await ctx.TaskSerials.AddRangeAsync(taskSerials).ConfigureAwait(false);
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            return true;
        }

        public async Task<List<string>> GetBins(long warhouseId)
        {
            var sql = $"select TRIM(WARHSNAME) from WAREHOUSES where WARHS = {warhouseId}";

            using var db = DbConnection;
            db.Open();
            var warhsname = await db.QuerySingleAsync<string>(sql).ConfigureAwait(false);

            sql = $@"SELECT LOCNAME
                FROM WAREHOUSES
                WHERE WARHSNAME = '{warhsname}'
                AND WARHS > 0
                AND LOCNAME<> '0' ";

            var results = await db.QueryAsync<string>(sql);

            return results.ToList();
        }

        public async Task<int> ActivateTask(long taskId, long userId)
        {
            using var ctx = Context;
            var task = await ctx.Tasks.FindAsync(taskId).ConfigureAwait(false);
            task.HWMS_ASSIGNUSER = userId;
            task.HWMS_ITASKSTATUS = "A";
            task.HWMS_TIMEASSIGNED = (long)DateTime.Now.Subtract(_baseDateTime).TotalMinutes;
            ctx.Entry(task).State = EntityState.Modified;
            var result = await ctx.SaveChangesAsync().ConfigureAwait(false);
            return result;
        }

        public async Task<int> DeleteTaskLots(long taskId)
        {
            using var db = DbConnection;
            var lots = await db.QuerySingleAsync<List<long>>($"select HWMS_ITASKLOT from HWMS_ITASKLOTS where HWMS_ITASK = {taskId}");
            await db.ExecuteAsync($"delete HWMS_ITASKSERIALS where HWMS_ITASKLOT in ({string.Join(",", lots)})");
            var sql = $"delete HWMS_ITASKLOTS where HWMS_ITASK = {taskId}";
            return await db.ExecuteAsync(sql).ConfigureAwait(false);

        }

        public async Task<List<SerialModel>> GetSerials(long partId, long serialId, string locName)
        {
            //priority bug of SERIAL
            var serial = "";
            if(serialId > 0) 
                serial = $" SERNUMBERS.SERIAL = {serialId} ";
            else
                serial = $" SERNUMBERS.SERIAL in (-1, 0) ";
            var sql = "";
            if (!string.IsNullOrEmpty(locName))
                sql = $@"SELECT  SERNUMBERS.SERN as Serial , SERNUMBERS.SERNUM AS SerialNumber , CUSTOMERS.CUSTNAME as [Status], {serialId} as Lot
            FROM    SERNUMBERS , WAREHOUSES ,CUSTOMERS
            WHERE   SERNUMBERS.PART = {partId}  
            AND     {serial}  
            AND     SERNUMBERS.SERN  > 0 
            AND     SERNUMBERS.WARHS  = WAREHOUSES.WARHS
            AND     SERNUMBERS.STATUS = CUSTOMERS.CUST
            AND     WAREHOUSES.LOCNAME = '{locName}'";
            else
                sql = $@"SELECT  SERNUMBERS.SERN as Serial , SERNUMBERS.SERNUM AS SerialNumber , CUSTOMERS.CUSTNAME as [Status], {serialId} as Lot
            FROM    SERNUMBERS , CUSTOMERS
            WHERE   SERNUMBERS.PART = {partId}  
            AND     {serial}  
            AND     SERNUMBERS.SERN  > 0 
            AND     SERNUMBERS.STATUS = CUSTOMERS.CUST";

            using var db = DbConnection;
            db.Open();
            var results = await db.QueryAsync<SerialModel>(sql).ConfigureAwait(false);

            return results.ToList();
        }
    }
}
