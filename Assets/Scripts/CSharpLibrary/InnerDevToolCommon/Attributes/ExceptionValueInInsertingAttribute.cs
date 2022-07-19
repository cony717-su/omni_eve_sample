using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevToolCommon.Attributes
{
    public class ExceptionValueInInsertingAttribute: BaseAttribute
    {
        public readonly object ExceptionValue;

        public ExceptionValueInInsertingAttribute(object value): base("Exception: {0}", value)
        {
            this.ExceptionValue = value;
        }
    }
}
