using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftup.CommonLib
{
    public class MessageException : Exception
    {
        public MessageException(string fmt, params object[] args)
            : base(String.Format(fmt, args))
        {
        }

        public MessageException(Exception inner, string fmt, params object[] args)
            : base(String.Format(fmt, args), inner)
        {
        }
    }

    public class NotImplementedForTypeExcetion : NotImplementedException
    {
        public NotImplementedForTypeExcetion(object obj)
            : base(String.Format("{0}: {1}에 대한 처리가 불가능합니다.", obj.GetType().Name, obj.ToString()))
        {
        }
        public NotImplementedForTypeExcetion(object obj, string fmt, params object[] args)
            : base(String.Format("{0}: {1}에 대한 처리가 불가능합니다. : ", obj.GetType().Name, obj.ToString()) + String.Format(fmt, args))
        {
        }
    }
}
