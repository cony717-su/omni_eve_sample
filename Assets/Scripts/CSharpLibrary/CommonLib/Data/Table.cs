using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Shiftup.CommonLib;
using Shiftup.CommonLib.Logger;
using Shiftup.CommonLib.Data.Attributes;

namespace Shiftup.CommonLib.Data
{
    using ConvertInfo = System.Tuple<Type, Type>;

    public sealed class Table
    {
        private const string delimiter = ",";

        public readonly TableMeta Meta;
        public readonly TableLog LoadingLog;

        private readonly Type rowType;
        private readonly IEnumerable<object> tableRows;
        private readonly List<DBLoader.ColumnInfo> usedFields = new List<DBLoader.ColumnInfo>();
        private readonly List<Tuple<FieldInfo, object>> defaultFields = new List<Tuple<FieldInfo, object>>();

        private delegate object ConvertTo(object from);

        static private Dictionary<ConvertInfo, ConvertTo> convertMap = new Dictionary<ConvertInfo, ConvertTo>();

        static Table()
        {
            convertMap.Add(new ConvertInfo(typeof(Double), typeof(Int32)), (from) => { return Convert.ToInt32(from); });
            convertMap.Add(new ConvertInfo(typeof(int), typeof(uint)), (from) => { return Convert.ToUInt32(from); });
            convertMap.Add(new ConvertInfo(typeof(sbyte), typeof(int)), (from) => { return Convert.ToInt32(from); });
        }
        public Table()
        {
            this.rowType = null;
            this.Meta = new TableMeta();
            this.tableRows = new List<object>();
            this.LoadingLog = new TableLog(String.Empty);
        }

        public Table(Type t, DBLoader loader, TableMeta meta)
        {
            this.rowType = t;
            this.Meta = meta;

            var defaultAttributes = GetAttributeInfo(new AttributeTypeSelector<DefaultValueAttribute>(), false)
                                            .ToDictionary(info => info.Field.Name, info => (info.Attribute as DefaultValueAttribute).Value);

            var skipped = new List<string>();
            foreach (var fi in t.GetFields())
            {
                var info = new DBLoader.ColumnInfo(fi);
                if (meta.FieldInfos.Keys.Contains(info.ColumnName) == true)
                {
                    usedFields.Add(info);
                }
                else
                {
                    skipped.Add(info.ColumnName);
                    if (defaultAttributes.ContainsKey(info.Field.Name) == true)
                    {
                        var value = defaultAttributes[info.Field.Name];
                        defaultFields.Add(new Tuple<FieldInfo, object>(fi, value));
                        Log.Debug("Table {0} has no {1} fields. Set with default {2}", meta.Name, info.ColumnName, value);
                    }
                    else
                    {
                        Log.Debug("Table {0} has no {1} fields. Skip to load", meta.Name, info.ColumnName);
                    }
                }
            }


            var rows = loadRows(loader, loader.BuildEntityName(t.Name));
            var duplicated = checkRows(rows);

            this.tableRows = rows;
            this.LoadingLog = new TableLog(meta.Name, usedFields.Select(info => info.ColumnName).ToList(), skipped, duplicated);
        }
        private Table(Table t, IEnumerable<object> rows)
        {
            this.rowType = t.rowType;
            this.Meta = t.Meta;
            this.LoadingLog = t.LoadingLog;
            this.usedFields = t.usedFields;
            this.defaultFields = t.defaultFields;
            this.tableRows = t.tableRows.Concat(rows).ToList();
        }

        public bool IsUsedField(string fieldName)
        {
            return usedFields.Where(info => info.Field.Name == fieldName).Any();
        }
        public IEnumerable<object> Rows()
        {
            foreach (var row in tableRows)
                yield return row;
        }

        public IEnumerable<T> Rows<T>()
        {
            if (checkTableType(typeof(T)) == false)
                throw new MessageException("테이블 컬럼({0})의 타입이 맞지 않습니다.", rowType.Name);

            foreach (var row in tableRows)
                yield return (T)row;
        }

        public void Export(string filename, bool writeColumnName, System.Text.Encoding encoding, bool ignoreNewline)
        {
            var file = new CSVFile(rowType, delimiter);

            file.AutoColumns();
            file.UseQuoteCharacter();
            if (ignoreNewline)
                file.IgnoreNewLine();
            file.SaveFile(filename, this.tableRows, true, encoding);
        }

        public Table ConcatRows<T>(IEnumerable<T> rows)
        {
            var newType = typeof(T);
            if (newType != this.rowType)
                throw new ArgumentException(String.Format("컬럼의 타입이 맞지 않습니다. {0} vs {1}.", newType.Name, this.rowType));

            return new Table(this, rows.Cast<object>());
        }

        public FieldInfo GetFieldInfo(string name, params object[] args)
        {
            if (args.Length > 0)
                name = String.Format(name, args);

            if (this.Meta.FieldInfos.Keys.Contains(name) == false)
                return null;

            return this.rowType.GetField(name);
        }
        public IEnumerable<AttributeInfo> GetAttributeInfo(AttributeSelector selector, bool databaseFieldsOnly)
        {
            var tableInfos = selector.SelectAttributes(rowType)
                                            .Select(attr => new AttributeInfo(attr, this));

            Func<FieldInfo, bool> fieldSelector;

            if (databaseFieldsOnly)
                fieldSelector = (fi) => this.Meta.FieldInfos.Keys.Contains((new DBLoader.ColumnInfo(fi)).ColumnName);
            else
                fieldSelector = (fi) => true;


            var fieldsInfos = this.rowType.GetFields()
                                            .Where(fi => fieldSelector(fi))
                                            .SelectMany(fi => selector.SelectAttributes(fi)
                                                                    .Select(attr => new AttributeInfo(attr, this, fi)));

            return tableInfos.Concat(fieldsInfos);
        }

        private IEnumerable<string> checkRows(IEnumerable<object> rows)
        {
            var fields = GetAttributeInfo(new AttributeTypeSelector<PrimaryKeyAttribute>(), true)
                                .OrderBy(info => (info.Attribute as PrimaryKeyAttribute).Order)
                                .Select(info => info.Field)
                                .ToList();

            if (fields.Count == 0)
                return Enumerable.Empty<string>();

            HashSet<string> duplicated = new HashSet<string>();
            var keySet = new HashSet<string>();
            foreach (var row in rows)
            {
                var key = String.Join("_", fields.Select(info => info.GetValue(row)));
                if (key.Length == 0)
                {
                    Log.Info("테이블({0})의 키 필드가 정의되지 않은 줄을 제외합니다.", this.Meta.Name);
                    continue;
                }

                if (keySet.Add(key) == false)
                {
                    Log.Error("테이블({0})의 키({1})이 중복되어 존재 합니다.", this.Meta.Name, key);
                    duplicated.Add(key);
                }
            }

            Log.Debug("Table {0} has {1} keys", this.Meta.Name, keySet.Count);
            return duplicated;
        }

        private bool checkTableType(Type t)
        {
            if (rowType == null)
                return true;

            if (t == rowType)
                return true;

            return rowType.IsSubclassOf(t);
        }

        private IEnumerable<object> loadRows(DBLoader loader, string tableName)
        {
            List<object> rows = new List<object>();
            foreach (var srcRow in loader.LoadTable(tableName, usedFields))
                rows.Add(buildRow(srcRow));

            return rows;
        }
        private object buildRow(IDictionary<string, Object> src)
        {
            var ret = Activator.CreateInstance(rowType);

            // data from default
            foreach (var tuple in defaultFields)
            {
                try
                {
                    setValue(tuple.Item1, ret, tuple.Item2);
                }
                catch (Exception inner)
                {
                    throw new MessageException(inner, "테이블({0})의 필드({1})에 값({2})을 설정할 수 없습니다.", rowType.Name, tuple.Item1, tuple.Item2.ToString());
                }
            }
            // data from DB table
            foreach (var info in usedFields)
            {
                if (src.ContainsKey(info.ColumnName) == false)
                    throw new MessageException("테이블({0})에 필드({1})가 없습니다.", rowType.Name, info.ColumnName);

                try
                {
                    setValue(info.Field, ret, src[info.ColumnName]);
                }
                catch (Exception inner)
                {
                    throw new MessageException(inner, "테이블({0})의 필드({1})에 값({2})을 설정할 수 없습니다.", rowType.Name, info.ColumnName, src[info.ColumnName].ToString());
                }
            }

            return ret;
        }

        private void setValue(FieldInfo fi, object value, object from)
        {
            if (Convert.IsDBNull(from) == true)
                from = getDefaultValue(fi);

            ConvertInfo info = new ConvertInfo(from.GetType(), fi.FieldType);
            if (info.Item1 != info.Item2)
            {
                if (convertMap.ContainsKey(info) == false)
                {
                    Log.Warning("{0}.{1} 데이터 타입({2} vs {3})이 일치 하지 않아서  값({4})을 적용할 수없습니다.", rowType.Name, fi.Name, fi.FieldType.ToString(), from.GetType().Name, from);
                    from = getDefaultValue(fi);
                }
                else
                {
                    from = convertMap[info](from);
                }
            }

            fi.SetValue(value, from);
        }

        private object getDefaultValue(FieldInfo fi)
        {
            if (fi.FieldType == typeof(String))
                return "";

            return Activator.CreateInstance(fi.FieldType);
        }
    }
}
