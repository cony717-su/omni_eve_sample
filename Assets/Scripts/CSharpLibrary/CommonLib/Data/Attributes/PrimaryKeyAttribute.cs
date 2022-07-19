using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib.Data.Attributes
{
    public class PrimaryKeyAttribute : TableBaseAttribute
    {
        public readonly int Order;
        public PrimaryKeyAttribute(int order = 0)
            : base(TableAttributeTypes.PrimaryKey)
        {
            this.Order = order;
        }
    }
}
