using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;
using Shiftup.CommonLib.Data.Bulk;

using InnerDevToolCommon.Attributes;
using InnerDevToolCommon.Database;

namespace InnerDevToolCommon.Data
{
    public abstract class ShardingRowData : RowData
    {
        [PrimaryKey]
        [CommonField]
        public ulong nfguid { get; set; }

        public static IEnumerable<InnerTableMeta> GetMetas(Type t, ulong nfguid)
        {
            var newMetas = new List<InnerTableMeta>();
            var metas = RowData.GetMetas(t);
            int shardKey = Convert.ToInt32(nfguid % 32);
            foreach (var meta in metas)
            {
                var tableName = String.Format("{0}_{1}", meta.Name, shardKey);
                newMetas.Add(new InnerTableMeta(tableName, meta.Fields.Keys, meta.PrimaryKeys, meta.AutoIncreaseKey, meta.JoinKeyInfos, meta.IsDefault, meta.ExceptionValues));
            }

            return newMetas;
        }

        public static IEnumerable<InnerTableMeta> GetShardingAllMetas(Type t)
        {
            var newMetas = new List<InnerTableMeta>();
            var metas = RowData.GetMetas(t);
            for (var i = 0; i < 32; i++)
            {
                foreach (var meta in metas)
                {
                    var tableName = String.Format("{0}_{1}", meta.Name, i);
                    newMetas.Add(new ShardingTableMeta(tableName, meta.Fields.Keys, meta.PrimaryKeys, meta.AutoIncreaseKey, meta.JoinKeyInfos, meta.IsDefault, meta.ExceptionValues, i));
                }
            }

            return newMetas;
        }

        public static string GetDefaultTableName(Type t, ulong nfguid)
        {
            int shardKey = Convert.ToInt32(nfguid % 32);
            return String.Format("{0}_{1}", RowData.GetDefaultTableName(t), shardKey);
        }
    }
}