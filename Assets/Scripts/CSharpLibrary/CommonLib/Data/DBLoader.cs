using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Common;

using Shiftup.CommonLib.Logger;

namespace Shiftup.CommonLib.Data
{
    public abstract class DBLoader
    {
        public class ColumnInfo
        {
            public readonly string ColumnName;
            public readonly FieldInfo Field;

            static private readonly ICollection<string> keywordSet = new HashSet<string>() {
                "object"
            };

            public ColumnInfo(FieldInfo fi)
            {
                this.Field = fi;
                this.ColumnName = fi.Name;
                if (fi.Name.EndsWith("_") == true)
                {
                    var colName = fi.Name.Substring(0, fi.Name.Length - 1);
                    if (keywordSet.Contains(colName) == true)
                        this.ColumnName = colName;
                }
            }
        }
        public abstract string Init(string connectionString);
        public abstract void Close();
        public abstract void PreLoad();
        public abstract void PostLoad();
        public abstract string BuildEntityName(string name);

        private ReadOnlyDictionary<string, TableMeta> metaDict = null;
        public ReadOnlyDictionary<string, TableMeta> GetMeta()
        {
            if (metaDict != null)
                return metaDict;

            try
            {
                this.PreLoad();
                metaDict = this.getTableMeta();
                this.PostLoad();
            }
            catch (Exception e)
            {
                Log.ErrorWithException(e, "데이터베이스 구조를 읽을수 없습니다.");
                return new ReadOnlyDictionary<string, TableMeta>();
            }
            return metaDict;
        }

        public IEnumerable<IDictionary<string, Object>> LoadTable(string tableName, IEnumerable<ColumnInfo> columns)
        {
            if (columns.Count() == 0)
                throw new MessageException("테이블({0})에 컬럼이 존재하지 않습니다.", tableName);

            var reader = executeSelectQuery(tableName, columns, String.Empty);
            if (reader == null)
                throw new MessageException("테이블({0})을 읽을 수 없습니다.", tableName);

            Log.Debug("Load Table {0}", tableName);

            foreach (var row in reader.Rows())
                yield return row.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            reader.Close();
        }

        protected abstract ReadOnlyDictionary<string, TableMeta> getTableMeta();
        protected abstract void executeInsertUpdateQuery(string query);
        protected abstract IRowReader executeSelectQuery(string tableName, IEnumerable<ColumnInfo> columns, string where);

    }
}
