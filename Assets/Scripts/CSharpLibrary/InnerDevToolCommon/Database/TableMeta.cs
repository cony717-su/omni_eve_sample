using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Bulk;

using InnerDevToolCommon.Attributes;

namespace InnerDevToolCommon.Database
{
    public class InnerTableMeta : TableMeta
    {
        public readonly string AutoIncreaseKey;
        public readonly IEnumerable<InnerAttributeInfo> JoinKeyInfos;
        public readonly bool IsDefault;
        public readonly IDictionary<string, object> ExceptionValues;

        public InnerTableMeta(string name, IEnumerable<string> allFields, IEnumerable<string> pks, string ak, IEnumerable<InnerAttributeInfo> joinKeys, bool isDefault, IDictionary<string, object> values)
            : base(name, allFields, pks)
        {
            this.AutoIncreaseKey = ak;
            this.JoinKeyInfos = joinKeys;
            this.IsDefault = isDefault;
            this.ExceptionValues = values;
        }
    }

    public class ShardingTableMeta : InnerTableMeta
    {
        public readonly int Shard;

        public ShardingTableMeta(string name, IEnumerable<string> allFields, IEnumerable<string> pks, string ak, IEnumerable<InnerAttributeInfo> joinKeys, bool isDefault, IDictionary<string, object> values, int shard)
            : base(name, allFields, pks, ak, joinKeys, isDefault, values)
        {
            this.Shard = shard;
        }
    }
}