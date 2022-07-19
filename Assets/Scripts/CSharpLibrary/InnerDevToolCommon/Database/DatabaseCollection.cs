using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerDevToolCommon.Database
{
    public static class DatabaseCollection
    {
        public static List<MysqlDatabase> Databases = new List<MysqlDatabase>();

        public static void Clear()
        {
            DatabaseCollection.Databases.Clear();
        }

        public static MysqlDatabase GetDatabase(string tableName)
        {
            foreach (var db in DatabaseCollection.Databases)
            {
                if (db.IsExistTable(tableName))
                {
                    return db;
                }
            }

            return null;
        }

        public static void Register(MysqlDatabase db)
        {
            DatabaseCollection.Databases.Add(db);
        }
    }
}