using HarelTech.WMS.Repository.Interfaces;
using HarelTech.WMS.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace HarelTech.WMS.Repository
{
    public class PriorityDbs: IPriorityDbs
    {
        //private readonly List<DbConnection> _dbConnections;

        public PriorityDbs(List<DatabaseConnection> dbConnections)
        {
            if (dbConnections == null || dbConnections.Count == 0)
                throw new ArgumentException("Empty Db connection list not allowed.");
            Dbs = new Dictionary<string, Priority>();
            foreach (var db in dbConnections)
            {
                Dbs.Add(db.DbName, new Priority(db.Connection));
            }
        }

        private Dictionary<string, Priority> Dbs { get; }

        public Priority CompanyDb(string company)
        {
            return Dbs[company];
        }
    }
}
