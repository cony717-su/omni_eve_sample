using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiftup.CommonLib;

using InnerDevToolCommon;
using InnerDevToolCommon.Common;
using InnerDevToolCommon.Database;

using InnerDevTool.Data;
using InnerDevTool.Data.Manager;

namespace InnerDevTool
{
    internal class DBLookup : Singleton<DBLookup>
    {
        public GameSql Game { get; set; }

        public MainSql Main { get; set; }

        //public Redis Redis { get; set; }

        public StaticData StaticData { get; set; }
    }
}