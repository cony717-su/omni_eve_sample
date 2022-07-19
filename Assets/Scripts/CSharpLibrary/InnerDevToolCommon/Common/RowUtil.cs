using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

using InnerDevToolCommon.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;

namespace InnerDevToolCommon.Common
{
    public static class RowUtil
    {
        private static Dictionary<KeyValuePair<Type, ulong>, IEnumerable<InnerTableMeta>> cacheMetas = new Dictionary<KeyValuePair<Type, ulong>, IEnumerable<InnerTableMeta>>();
        private static Dictionary<Type, IEnumerable<string>> cachePks = new Dictionary<Type, IEnumerable<string>>();

        public static string GetDefaultTableName(Type t, ulong nfguid = 0)
        {
            string tableName = String.Empty;
            if (t.IsSubclassOf(typeof(ShardingRowData)))
            {
                tableName = ShardingRowData.GetDefaultTableName(t, nfguid);
            }
            else if (t.IsSubclassOf(typeof(RowData)))
            {
                tableName = RowData.GetDefaultTableName(t);
            }
            return tableName;
        }

        public static IEnumerable<InnerTableMeta> GetMetas(Type t, ulong nfguid = 0)
        {
            var key = new KeyValuePair<Type, ulong>(t, nfguid);
            if (cacheMetas.ContainsKey(key))
            {
                return cacheMetas[key];
            }

            IEnumerable<InnerTableMeta> metas = Enumerable.Empty<InnerTableMeta>();
            if (t.IsSubclassOf(typeof(ShardingRowData)))
            {
                if (nfguid == 0)
                {
                    metas = ShardingRowData.GetShardingAllMetas(t);
                }
                else
                {
                    metas = ShardingRowData.GetMetas(t, nfguid);
                }
            }
            else if (t.IsSubclassOf(typeof(RowData)))
            {
                metas = RowData.GetMetas(t);
            }

            cacheMetas[key] = metas;

            return metas;
        }

        public static IEnumerable<string> GetPrimaryKeys(Type t)
        {
            if (!cachePks.ContainsKey(t))
            {
                cachePks[t] = AttributeUtil.SelectPropertyNames<PrimaryKeyAttribute>(t);
            }

            return cachePks[t];
        }
    }
}