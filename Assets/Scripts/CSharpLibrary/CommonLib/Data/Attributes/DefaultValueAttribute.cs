using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib.Data.Attributes
{
    public class DefaultValueAttribute : TableBaseAttribute
    {
        public readonly object Value;
        public DefaultValueAttribute(object value)
            : base(TableAttributeTypes.DefaultValue)
        {
            this.Value = value;
        }
    }
}
