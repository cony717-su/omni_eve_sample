using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib.Data.Attributes;

namespace InnerDevToolCommon.Attributes
{
    public class JoinPrimaryKeyAttribute : BaseAttribute
    {
        public string TableName { get; protected set; }
        public string ColumnName { get; protected set; }
        public JoinPrimaryKeyAttribute(string tableName, string columnName) : base("JoinPrimaryKey : {0} - {1}", tableName, columnName) 
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
        }
    }
}
