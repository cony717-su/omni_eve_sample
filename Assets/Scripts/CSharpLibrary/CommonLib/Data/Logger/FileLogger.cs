using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shiftup.CommonLib.Logger
{
    class FileLogger : LogWriter
    {
        private readonly StreamWriter file = null;
        private readonly string filename;
        public FileLogger(string filename, Levels l)
            : base(Types.File, l)
        {
            this.file = new StreamWriter(filename);
            this.filename = filename;
        }


        public override string Description { get { return String.Format("FileLogger {0}", this.filename); } }
        public override void AddLine(string ts, Levels lvl, string msg)
        {
            file.WriteLine("{0}({1}) - {2}", ts, lvl.ToString(), msg);
        }

        public override void Flush()
        {
            file.Flush();
        }


        public override void Close()
        {
            if (file != null)
            {
                file.WriteLine("File logger is closed.");
                file.Flush();
                file.Close();
            }
        }
    }
}
