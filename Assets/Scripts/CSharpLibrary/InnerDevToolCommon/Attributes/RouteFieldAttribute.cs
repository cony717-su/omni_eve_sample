using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevToolCommon.Attributes
{
    public class RouteFieldAttribute : BaseAttribute
    {
        public RouteFieldAttribute(string tableName) : base(tableName)
        { }
    }
}