using System;
using System.Collections.Generic;
using System.Text;

namespace Shiftup.CommonLib.Logger
{
    public abstract class LogWriter
    {
        protected LogWriter(Types t, Levels l)
        {
            LoggerType = t;
            Level = l;
        }
        public abstract void AddLine(string ts, Levels lvl, string msg);
        public abstract void Flush();
        public abstract void Close();

        public Levels Level;
        public Types LoggerType { get; private set; }

        public abstract string Description { get; }
    }
}
