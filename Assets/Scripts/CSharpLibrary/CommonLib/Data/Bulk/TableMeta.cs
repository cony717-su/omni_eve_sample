using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Logger;

namespace Shiftup.CommonLib.Data.Bulk
{
    [DebuggerDisplay("Table : {Name}")]

    public class TableMeta
    {
        public readonly string Name;
        public readonly ReadOnlyDictionary<string, ColumnInfo> Fields;
        public IEnumerable<string> PrimaryKeys;
        public readonly bool IsMultiPK;

        public class ColumnInfo
        {
            public readonly string Name;
            public readonly bool IsPK;
            public TypeCode TypeCode { get { return Type.GetTypeCode(this.DataType); } }
            public Type DataType { get; private set; }

            public ColumnInfo(string name, bool primaryKey)
                : this(name, null, primaryKey)
            {
            }

            public ColumnInfo(string name, Type t, bool isPK)
            {
                this.Name = name;
                this.DataType = t;
                this.IsPK = isPK;
            }

            public void UpdateType(Type t)
            {
                this.DataType = t;
            }
        }

        public TableMeta(string name, IEnumerable<string> allFields, IEnumerable<string> pks)
        {
            var primaryKeys = new HashSet<string>(pks);

            this.Name = name;
            this.Fields = allFields
                            .ToDictionary(
                                f => f,
                                f => new ColumnInfo(f, primaryKeys.Contains(f)))
                            .AsReadOnly();
            this.PrimaryKeys = pks;
            this.IsMultiPK = pks.Count() > 1;
        }
        public void UpdateTypeCode(DataRowCollection rows)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var name = row["ColumnName"] as string;
                var type = row["DataType"] as Type;

                if (this.Fields.ContainsKey(name) == false)
                    throw new MessageException("컬럼({0}) 정보가 맞지 않습니다.", name);

                this.Fields[name].UpdateType(type);
            }
        }

        public void WriteCSVHeader(StreamWriter sw, char delimiter)
        {
            bool first = true;
            foreach (var field in Fields.Keys)
            {
                if (first)
                    first = false;
                else
                    sw.Write(delimiter);

                sw.Write(field);
            }
            sw.WriteLine();
        }

        public bool CheckCSVHeader(string[] headerColumns, bool exactlyColumns)
        {
            if (headerColumns == null)
            {
                Log.Error("CSV파일({0})의 컬럼 헤더가 없습니다.", this.Name);
                return false;
            }

            var tableOnly = this.Fields.Keys.Except(headerColumns);
            if (tableOnly.Any())
            {
                Log.Error("CSV파일({0})에 데이터베이스 컬럼({1})이 없습니다.", this.Name, String.Join(",", tableOnly));
                return false;
            }

            Levels level;
            bool checkResult;

            var csvOnly = headerColumns.Except(this.Fields.Keys);
            bool csvOnlyExist = csvOnly.Any();

            if (exactlyColumns)
            {
                level = Levels.Error;
                checkResult = csvOnlyExist == false;
            }
            else
            {
                level = Levels.Warning;
                checkResult = true;
            }

            if (csvOnlyExist)
                Log.Write(level, "CSV 파일({0})의 컬럼({1})이 데이터베이스에는 존재하지 않습니다.", this.Name, String.Join(",", csvOnly));

            return checkResult;
        }
    }
}
