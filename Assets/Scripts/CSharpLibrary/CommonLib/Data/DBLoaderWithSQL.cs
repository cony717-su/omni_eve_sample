using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;

using Shiftup.CommonLib.Logger;

namespace Shiftup.CommonLib.Data
{

    public abstract class DBLoaderWithSQL : DBLoader
    {
        protected string metaTable;
        protected string[] metaFields;
        protected string metaWhere;
        protected ReadOnlyDictionary<string, TypeCode> typeNameLookup;

        private string buildSelectQuery(string tableName, IEnumerable<string> cols, string where)
        {
            var columns = String.Join(",", cols);
            if (String.IsNullOrWhiteSpace(where))
                return String.Format("SELECT {0} FROM {1}", columns, tableName);
            else
                return String.Format("SELECT {0} FROM {1} WHERE {2}", columns, tableName, where);
        }

        protected override sealed IRowReader executeSelectQuery(string tableName, IEnumerable<ColumnInfo> columns, string where)
        {
            string query = buildSelectQuery(tableName, columns.Select(c => BuildEntityName(c.ColumnName)), where);
            return new DBRowReader(getCommand(query).ExecuteReader());
        }

        protected override sealed void executeInsertUpdateQuery(string query)
        {
            getCommand(query).ExecuteNonQuery();
        }
        protected override sealed ReadOnlyDictionary<string, TableMeta> getTableMeta()
        {
            string query = buildSelectQuery(metaTable, metaFields, metaWhere);
            var reader = getCommand(query).ExecuteReader();

            List<Tuple<string, string, string>> fields = new List<Tuple<string, string, string>>();
            while (reader.Read())
            {
                var table = reader.GetValue(0).ToString();
                var field = reader.GetValue(1).ToString();
                var type = reader.GetValue(2).ToString();

                fields.Add(new Tuple<string, string, string>(table, field, type));
            }

            reader.Close();

            var x = fields.Select(t => t.Item3).Distinct().ToList();

            return fields.GroupBy(tuple => tuple.Item1)
                    .ToDictionary(
                        group => group.Key,
                        group => new TableMeta(group.Key, group.ToDictionary(tuple => tuple.Item2, tuple => getTypeName(tuple.Item3))))
                    .AsReadOnly();
        }

        protected abstract DbCommand getCommand(string query);

        private TypeCode getTypeName(string typeName)
        {
            try
            {
                var splits = typeName.Split(new char[] { '(' });
                return typeNameLookup[splits[0]];
            }
            catch (Exception)
            {
                Log.Error("타입{0}의 데이터 타입을 알 수 없습니다.", typeName);
                return TypeCode.Empty;
            }
        }
    }
}
