using System;
using System.Diagnostics;

namespace Shiftup.CommonLib.Logger
{
    public class DebugOutputLogger : LogWriter
    {
        public DebugOutputLogger(Levels l)
            : base(Types.DebugOutput, l)
        {
        }

        public override void AddLine(string ts, Levels lvl, string msg)
        {
            Debug.WriteLine("{0}({1}) {2}", ts, lvl.ToString(), msg);
        }

        public override void Flush()
        {
        }

        public override string Description { get { return "DebugOutputLogger"; } }
        public override void Close()
        {
            Debug.WriteLine("Debug Output log is closed.");
        }
    }
}
