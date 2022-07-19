using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib.Data.Attributes
{
    public enum TableAttributeTypes
    {
        PrimaryKey,
        DefaultValue,
    }
    public class TableBaseAttribute : BaseAttribute
    {
        public readonly TableAttributeTypes TableAttributeType;
        protected TableBaseAttribute(TableAttributeTypes t)
            : base("TableBase Attribute: {0}", t)
        {
            this.TableAttributeType = t;
        }
    }
}
