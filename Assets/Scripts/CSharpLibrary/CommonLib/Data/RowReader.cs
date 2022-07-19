using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace Shiftup.CommonLib.Data
{
    using Row = IEnumerable<KeyValuePair<string, Object>>;
    public interface IRowReader
    {
        IEnumerable<Row> Rows();
        bool IsValid { get; }
        void Close();
    }
}
