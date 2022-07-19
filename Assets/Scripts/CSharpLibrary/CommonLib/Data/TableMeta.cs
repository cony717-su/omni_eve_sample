using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shiftup.CommonLib.Data
{
    public class TableMeta
    {
        public readonly string Name;
        public readonly ReadOnlyDictionary<string, TypeCode> FieldInfos;

        public TableMeta()
            : this(String.Empty, new Dictionary<string, TypeCode>())
        {
        }
        public TableMeta(string name, Dictionary<string, TypeCode> fields)
        {
            this.Name = name;
            this.FieldInfos = fields.AsReadOnly();
        }

        public bool CheckColumns(Type t)
        {
            var typeFields = t.GetFields().Select(fi => fi.Name);

            return this.FieldInfos.Keys.Intersect(typeFields).Any();
        }
    }
}
