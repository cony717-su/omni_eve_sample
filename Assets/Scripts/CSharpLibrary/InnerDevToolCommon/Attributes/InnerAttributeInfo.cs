using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevToolCommon.Attributes
{
    public class InnerAttributeInfo
    {
        public readonly BaseAttribute Attribute;
        public readonly PropertyInfo Property;

        public InnerAttributeInfo(BaseAttribute ba, PropertyInfo pi)
        {
            this.Attribute = ba;
            this.Property = pi;
        }
    }
}