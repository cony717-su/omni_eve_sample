using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Shiftup.CommonLib.Logger;

using InnerDevToolCommon.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;
using InnerDevToolCommon.Common;

namespace InnerDevToolCommon
{
    public class DatabaseApplier : Singleton<DatabaseApplier>
    {
        static private readonly ICollection<string> sqlKeywords = new HashSet<string>() { "interval", "index" };

        public delegate void SetLastIdDelegate<T>(InnerTableMeta meta, T row, long lastId);

        private IEnumerable<KeyValuePair<string, object>> BuildParameters<T>(T row, IEnumerable<string> fields) where T : RowData
        {
            return fields.Select(f => new KeyValuePair<string, object>(GetFieldParameterName(f), ToMySqlQueryValue(row[f], row[f, true])));
        }

        private string BuildParametersNameAndValue(string joinString, string tableName, IEnumerable<string> fields)
        {
            return String.Join(joinString, fields.Select(f =>
                String.Format("{0}.{1} = {2}", tableName, QuoteForKeyword(f), GetFieldParameterName(f))));
        }

        private string GetFieldParameterName(string field)
        {
            return String.Format("@field_{0}", field);
        }

        private string GetSelectQuery(string selectClause, string fromClause, string whereClause, string orderDescColumnName, int limit)
        {
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                whereClause = String.Format("WHERE {0}", whereClause);
            }
            string orderClause = "";
            if (!String.IsNullOrWhiteSpace(orderDescColumnName))
            {
                orderClause = String.Format("ORDER BY {0} DESC", orderDescColumnName);
            }
            string limitClause = "";
            if (limit != -1)
            {
                limitClause = String.Format("LIMIT {0}", limit);
            }

            return String.Format(
                "SELECT {0} FROM {1} {2} {3} {4}",
                selectClause,
                fromClause,
                whereClause,
                orderClause,
                limitClause);
        }

        private string GetWhereConditionClause(string tableName, IEnumerable<string> parameterNames)
        {
            return String.Join(" AND ", parameterNames.Select(p => String.Format("{0}.{1} = {2}",
                tableName,
                QuoteForKeyword(p),
                GetFieldParameterName(p))));
        }

        private string QuoteForKeyword(string field)
        {
            if (sqlKeywords.Contains(field))
                return String.Format("`{0}`", field);

            return field;
        }

        private object ToMySqlQueryValue(object obj, Type objType)
        {
            if (objType == typeof(DateTime))
            {
                DateTime dt = (DateTime)obj;
                return String.Format(
                    "{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}",
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour,
                    dt.Minute,
                    dt.Second);
            }
            else if (objType == typeof(DateTime?))
            {
                if (obj == null)
                {
                    return "0000-00-00 00:00:00";
                }

                DateTime dt = (DateTime)obj;
                return String.Format(
                    "{0:D4}-{1:D2}-{2:D2}T{3:D2}:{4:D2}:{5:D2}",
                    dt.Year,
                    dt.Month,
                    dt.Day,
                    dt.Hour,
                    dt.Minute,
                    dt.Second);
            }
            else if (objType == typeof(string))
            {
                if (obj == null)
                {
                    return "";
                }
            }

            return obj;
        }

        public void InsertRow<T>(T row, InnerTableMeta meta, SetLastIdDelegate<T> callback) where T : RowData
        {
            var database = DatabaseCollection.GetDatabase(meta.Name);
            if (database != null)
            {
                var databaseFields = database.GetTableMeta(meta.Name).FieldInfos.Keys;
                var fields = databaseFields.Intersect(meta.Fields.Keys);
                if (fields.Count() == 0)
                {
                    fields = meta.Fields.Keys;
                }

                var values = meta.ExceptionValues;
                foreach (var pair in values)
                {
                    if (!fields.Contains(pair.Key))
                    {
                        continue;
                    }

                    if (row[pair.Key].Equals(pair.Value))
                    {
                        return;
                    }
                }

                string query = String.Format(
                    "INSERT INTO {0} ({1}) VALUES ({2})",
                    meta.Name,
                    String.Join(",", fields.Select(key => QuoteForKeyword(key))),
                    String.Join(",", fields.Select(f => GetFieldParameterName(f))));

                var cmd = database.QueryWithNoneReader(query, BuildParameters(row, fields));
                if (callback != null)
                {
                    callback(meta, row, cmd.LastInsertedId);
                }
            }
        }

        public void InsertOnDuplicateKeyUpdateRow<T>(T row, InnerTableMeta meta, SetLastIdDelegate<T> callback) where T : RowData
        {
            var database = DatabaseCollection.GetDatabase(meta.Name);
            if (database != null)
            {
                var databaseFields = database.GetTableMeta(meta.Name).FieldInfos.Keys;
                var fields = databaseFields.Intersect(meta.Fields.Keys);
                if (fields.Count() == 0)
                {
                    fields = meta.Fields.Keys;
                }

                var values = meta.ExceptionValues;
                foreach (var pair in values)
                {
                    if (!fields.Contains(pair.Key))
                    {
                        continue;
                    }

                    if (row[pair.Key].Equals(pair.Value))
                    {
                        return;
                    }
                }

                string query = String.Format(
                    "INSERT INTO {0} ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3}",
                    meta.Name,
                    String.Join(",", fields.Select(key => QuoteForKeyword(key))),
                    String.Join(",", fields.Select(f => GetFieldParameterName(f))),
                    BuildParametersNameAndValue(", ", meta.Name, fields));

                var cmd = database.QueryWithNoneReader(query, BuildParameters(row, fields));
                if (callback != null)
                {
                    callback(meta, row, cmd.LastInsertedId);
                }
            }
        }

        public void AddRows<T>(IEnumerable<InnerTableMeta> metas, IEnumerable<T> rows, SetLastIdDelegate<T> callback) where T : RowData
        {
            if (rows.Any() == false)
            {
                return;
            }

            // auto key 로 인해 먼저 defualt meta 부터 넣어야 함
            var defaultMeta = metas.First(meta => meta.IsDefault);
            foreach (var row in rows)
            {
                InsertRow(row, defaultMeta, callback);
                foreach (var meta in metas.Where(meta => !meta.IsDefault))
                {
                    InsertRow(row, meta, callback);
                }
            }
        }

        public void AddOrUpdateRows<T>(IEnumerable<InnerTableMeta> metas, IEnumerable<T> rows, SetLastIdDelegate<T> callback = null) where T : RowData
        {
            if (rows.Any() == false)
            {
                return;
            }

            // auto key 로 인해 먼저 defualt meta 부터 넣어야 함
            var defaultMeta = metas.First(meta => meta.IsDefault);
            foreach (var row in rows)
            {
                InsertOnDuplicateKeyUpdateRow(row, defaultMeta, callback);
                foreach (var meta in metas.Where(meta => !meta.IsDefault))
                {
                    InsertOnDuplicateKeyUpdateRow(row, meta, callback);
                }
            }
        }

        public void DeleteRows<T>(IEnumerable<InnerTableMeta> metas, IEnumerable<T> rows) where T : RowData
        {
            if (rows.Any() == false)
            {
                return;
            }

            foreach (var row in rows)
            {
                foreach (var meta in metas)
                {
                    var database = DatabaseCollection.GetDatabase(meta.Name);
                    if (database == null)
                    {
                        continue;
                    }

                    string query = String.Format(
                    "DELETE FROM {0} WHERE {1}",
                    meta.Name,
                    BuildParametersNameAndValue(" AND ", meta.Name, meta.PrimaryKeys));

                    database.QueryWithNoneReader(query, BuildParameters(row, meta.PrimaryKeys));
                }
            }
        }

        public void DeleteTable(string tableName, ulong nfguid)
        {
            string query = String.Format(
                "DELETE FROM {0} WHERE nfguid = @nfguid",
                tableName);
            var database = DatabaseCollection.GetDatabase(tableName);
            if (database == null)
            {
                return;
            }
            database.QueryWithNoneReader(query, new KeyValuePair<string, object>("@nfguid", nfguid));
        }

        public ulong GetAutoIncrement<T>(InnerTableMeta meta)
        {
            var database = DatabaseCollection.GetDatabase(meta.Name);
            if (database == null)
            {
                return 0;
            }

            string query = String.Format(
                "SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}'",
                database.Name,
                meta.Name);
            return (ulong)database.QueryWithScalar(query);
        }

        public IEnumerable<IDictionary<string, object>> SelectRows(string tableName, IDictionary<string, object> parameters = null, string orderDescColumnName = null, int limit = -1)
        {
            var database = DatabaseCollection.GetDatabase(tableName);
            if (database == null)
            {
                Log.Warning("{0} table does not exist!", tableName);
                return Enumerable.Empty<IDictionary<string, object>>();
            }

            var tableMeta = database.GetTableMeta(tableName);
            string whereClause = String.Empty;
            if (parameters != null)
            {
                whereClause = BuildParametersNameAndValue(" AND ", tableName, parameters.Keys.Where(key => tableMeta.FieldInfos.Keys.Contains(key)));
            }
            string query = GetSelectQuery("*", tableName, whereClause, orderDescColumnName, limit);

            try
            {
                if (parameters == null)
                {
                    return database.QueryWithReader(query);
                }
                else
                {
                    return database.QueryWithReader(query, parameters.Select(p => new KeyValuePair<string, object>(GetFieldParameterName(p.Key), p.Value)));
                }
            }
            catch (Exception e)
            {
                Log.Warning("{0} query error {1}", query, e.Message);
                return Enumerable.Empty<IDictionary<string, object>>();
            }
        }

        public IEnumerable<IDictionary<string, object>> SelectRows(Type t, IEnumerable<InnerTableMeta> metas, IDictionary<string, object> parameters = null, string orderDescColumnName = null, int limit = -1)
        {
            metas = metas.Where(meta => DatabaseCollection.GetDatabase(meta.Name) != null);
            if (metas.Any() == false)
            {
                return Enumerable.Empty<IDictionary<string, object>>();
            }

            // nfguid 있으면 sharding
            ulong nfguid = 0;
            if (parameters != null && parameters.ContainsKey("nfguid"))
            {
                nfguid = (ulong)parameters["nfguid"];
            }
            string baseTableName = RowUtil.GetDefaultTableName(t, nfguid);
            var database = DatabaseCollection.GetDatabase(baseTableName);
            if (database == null)
            {
                return Enumerable.Empty<IDictionary<string, object>>();
            }
            string fromClause = "";

            // 각 테이블에 중복되는 pk 있는지 확인
            var commonPks = Enumerable.Empty<string>();
            if (metas.Count() > 1)
            {
                commonPks = metas.Skip(1).Aggregate(new HashSet<string>(metas.First().PrimaryKeys), (h, e) => { h.IntersectWith(e.PrimaryKeys); return h; });
            }

            var fromClauseList = new List<string>() { baseTableName };
            var joinKeyInfoByTable = metas
                .SelectMany(meta => meta.JoinKeyInfos)
                .GroupBy(item => (item.Attribute as JoinPrimaryKeyAttribute).TableName)
                .ToDictionary(group => group.Key, group => (IEnumerable<InnerAttributeInfo>)group);
            if (joinKeyInfoByTable.Count > 0)
            {
                // join key 정보를 통해 natural join
                fromClauseList.AddRange(
                    joinKeyInfoByTable.Select(pair => String.Format("{0} ON ({1})", pair.Key, String.Join(" AND ", pair.Value.Select(attrInfo =>
                    {
                        var attr = attrInfo.Attribute as JoinPrimaryKeyAttribute;
                        return String.Format("{0}.{1} = {2}.{3}",
                            baseTableName,
                            attrInfo.Property.Name,
                            attr.TableName,
                            attr.ColumnName);
                    }).Union(commonPks.Select(pk => String.Format("{0}.{1} = {2}.{1}", baseTableName, pk, pair.Key))))))
                );
                fromClause = String.Join(" LEFT OUTER JOIN ", fromClauseList);
            }
            else
            {
                if (commonPks.Count() > 0)
                {
                    // common pk 있으면 join
                    fromClauseList.AddRange(
                        metas.Where(meta => !meta.Name.Equals(baseTableName))
                        .Select(meta =>
                            String.Format("{0} ON ({1})",
                                meta.Name,
                                String.Join(" AND ", commonPks.Select(pk => String.Format("{0}.{1} = {2}.{1}", baseTableName, pk, meta.Name)))))
                    );
                    fromClause = String.Join(" LEFT OUTER JOIN ", fromClauseList);
                }
                else
                {
                    fromClause = metas.ElementAt(0).Name;
                }
            }

            var whereClause = String.Empty;
            if (parameters != null)
            {
                var conditionList = metas.Select(meta =>
                {
                    var fields = meta.Fields.Where(fi => parameters.Keys.Contains(fi.Value.Name));
                    if (!meta.IsDefault)
                    {
                        fields = fields.Where(fi => !commonPks.Contains(fi.Value.Name));
                    }

                    if (fields.Count() == 0)
                    {
                        return String.Empty;
                    }

                    return String.Format("({0})", BuildParametersNameAndValue(
                        " AND ",
                        meta.Name,
                        fields.Select(fi => fi.Value.Name)
                    ));
                });
                whereClause = String.Join(" OR ", conditionList.Where(condition => !String.IsNullOrWhiteSpace(condition)));
            }
            var selectColumns = String.Join(
                ",",
                metas.SelectMany(meta =>
                {
                    var tableMeta = database.GetTableMeta(meta.Name);
                    if (tableMeta == null)
                    {
                        return Enumerable.Empty<string>();
                    }
                    var databaseFields = tableMeta.FieldInfos.Keys;

                    var tableFields = meta.Fields.Values.AsEnumerable();
                    if (!meta.IsDefault)
                    {
                        tableFields = tableFields.Where(fi => !commonPks.Contains(fi.Name));
                    }
                    var selectFields = tableFields.Select(fi => fi.Name).Union(Enumerable.Repeat(meta.AutoIncreaseKey, 1)).Where(key => !String.IsNullOrWhiteSpace(key));
                    var fields = databaseFields.Intersect(selectFields);
                    if (fields.Count() == 0)
                    {
                        fields = selectFields;
                    }
                    return fields.Select(key => String.Format("{0}.{1}", meta.Name, QuoteForKeyword(key)));
                })
            );
            string query = GetSelectQuery(selectColumns, fromClause, whereClause, orderDescColumnName, limit);

            try
            {
                if (parameters == null)
                {
                    return database.QueryWithReader(query);
                }
                else
                {
                    return database.QueryWithReader(query, parameters.Select(p => new KeyValuePair<string, object>(GetFieldParameterName(p.Key), p.Value)));
                }
            }
            catch (Exception e)
            {
                Log.Warning("{0} query error, {1}", query, e.Message);
                return Enumerable.Empty<IDictionary<string, object>>();
            }
        }

        public void UpdateRows<T>(IEnumerable<InnerTableMeta> metas, IEnumerable<T> rows) where T : RowData
        {
            if (rows.Any() == false)
            {
                return;
            }

            foreach (var row in rows)
            {
                foreach (var meta in metas)
                {
                    var database = DatabaseCollection.GetDatabase(meta.Name);
                    if (database == null)
                    {
                        continue;
                    }

                    var databaseFields = database.GetTableMeta(meta.Name).FieldInfos.Keys;
                    var fields = databaseFields.Intersect(meta.Fields.Where(kvp => kvp.Value.IsPK == false).Select(kvp => kvp.Key));
                    if (fields.Count() == 0)
                    {
                        fields = meta.Fields.Where(kvp => kvp.Value.IsPK == false).Select(kvp => kvp.Key);
                    }

                    var exception = false;
                    var values = meta.ExceptionValues;
                    foreach (var pair in values)
                    {
                        if (!fields.Contains(pair.Key))
                        {
                            continue;
                        }

                        // 키가 존재하며 attribute value 와 update value 가 같지 않을 때 insert into on duplicated update.
                        if (!row[pair.Key].Equals(pair.Value))
                        {
                            exception = true;
                        }
                    }

                    string query = String.Format(
                        "UPDATE {0} SET {1} WHERE {2}",
                        meta.Name,
                        BuildParametersNameAndValue(", ", meta.Name, fields),
                        BuildParametersNameAndValue(" AND ", meta.Name, meta.PrimaryKeys));

                    if (exception)
                    {
                        var allFields = databaseFields.Intersect(meta.Fields.Keys);
                        if (allFields.Count() == 0)
                        {
                            allFields = meta.Fields.Keys;
                        }
                        query = String.Format(
                            "INSERT INTO {0} ({1}) VALUES ({2}) ON DUPLICATE KEY UPDATE {3}",
                            meta.Name,
                            String.Join(",", allFields.Select(key => QuoteForKeyword(key))),
                            String.Join(",", allFields.Select(f => GetFieldParameterName(f))),
                            String.Join(",", BuildParametersNameAndValue(", ", meta.Name, fields)));
                    }

                    database.QueryWithNoneReader(query, BuildParameters(row, meta.Fields.Keys.Union(meta.PrimaryKeys)));
                }
            }
        }

        public void UpdateRowsByNfguid(string tableName, string setClause, string whereClause, ulong nfguid)
        {
            var database = DatabaseCollection.GetDatabase(tableName);
            if (database == null)
            {
                return;
            }

            var meta = database.GetTableMeta(tableName);
            List<string> whereConditions = new List<string>();
            if (meta.FieldInfos.ContainsKey("nfguid"))
            {
                whereConditions.Add("nfguid = @nfguid");
            }
            if (!String.IsNullOrWhiteSpace(whereClause))
            {
                whereConditions.Add(whereClause);
            }

            string where = String.Join(" AND ", whereConditions);
            string query = String.Format(
                "UPDATE {0} SET {1} WHERE {2}",
                tableName,
                setClause,
                where);
            database.QueryWithNoneReader(query, new KeyValuePair<string, object>("@nfguid", nfguid));
        }
    }
}