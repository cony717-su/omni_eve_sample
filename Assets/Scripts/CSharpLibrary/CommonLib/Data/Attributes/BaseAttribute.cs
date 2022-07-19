using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib.Data.Attributes
{
    public class BaseAttribute : Attribute
    {
        public readonly string Name;
        protected BaseAttribute(string fmt, params object[] args)
        {
            this.Name = String.Format(fmt, args);
        }
    }
}
