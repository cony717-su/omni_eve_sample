using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib.Data.Attributes
{
    public class AttributeInfo
    {
        public readonly BaseAttribute Attribute;
        public readonly Table Table;
        public readonly FieldInfo Field;
        public readonly bool IsTableAttribute;
        public IEnumerable<FieldInfo> RefFields { get; private set; }
        public AttributeInfo(BaseAttribute ba, Table table)
            : this(ba, table, null)
        {
            this.RefFields = Enumerable.Empty<FieldInfo>();
        }
        public AttributeInfo(BaseAttribute ba, Table table, FieldInfo fi)
        {
            this.Attribute = ba;
            this.Table = table;
            this.Field = fi;
            this.RefFields = Enumerable.Empty<FieldInfo>();

            if (fi == null)
                this.IsTableAttribute = true;
            else
                this.IsTableAttribute = false;
        }

        public void UpdateRefFields(IDictionary<string, IEnumerable<FieldInfo>> refDict)
        {
            if (refDict.ContainsKey(this.Table.Meta.Name) == false)
                this.RefFields = Enumerable.Empty<FieldInfo>();
            else
                this.RefFields = refDict[this.Table.Meta.Name];
        }

        public override string ToString()
        {
            if (IsTableAttribute)
                return String.Format("{0}:{1}", Table.Meta.Name, Attribute.Name);
            else
                return String.Format("{0}.{1}:{2}", Table.Meta.Name, Field.Name, Attribute.Name);
        }
    }
}
