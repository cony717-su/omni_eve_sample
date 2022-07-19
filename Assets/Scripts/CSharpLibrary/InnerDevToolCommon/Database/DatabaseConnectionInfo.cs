using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerDevToolCommon.Database
{
    public class DBConnectionInfo
    {
        public string DatabaseID { get; set; }
        public string DatabaseProfileName { get; set; }

        public string DatabasePW { get; set; }
        public string GameDatabaseHostInfo { get; set; }
        public string GameDatabaseName { get; set; }
        public string IsQA { get; set; }
        public string MainDatabaseHostInfo { get; set; }
        public string MainDatabaseName { get; set; }
        public string ServerUrl { get; set; }
    }
}
