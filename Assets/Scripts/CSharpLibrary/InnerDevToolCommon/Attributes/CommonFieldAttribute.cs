using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevToolCommon.Attributes
{
    public class CommonFieldAttribute : BaseAttribute
    {
        public CommonFieldAttribute() : base("CommonField")
        { }

    }
}