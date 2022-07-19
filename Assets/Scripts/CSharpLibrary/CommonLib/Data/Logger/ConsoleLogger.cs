using System;
using System.Collections.Generic;
using System.Text;

namespace Shiftup.CommonLib.Logger
{
    class ConsoleLogger : LogWriter
    {
        static public readonly Dictionary<Levels, ConsoleColor> Colors = new Dictionary<Levels, ConsoleColor>();

        static ConsoleLogger()
        {
            Colors[Levels.Debug] = ConsoleColor.DarkGray;
            Colors[Levels.Info] = ConsoleColor.White;
            Colors[Levels.Warning] = ConsoleColor.Yellow;
            Colors[Levels.Error] = ConsoleColor.Red;
        }

        public ConsoleLogger(Levels l)
            : base(Types.Console, l)
        {
        }
        public override void AddLine(string ts, Levels lvl, string msg)
        {
            Console.ForegroundColor = Colors[lvl];
            Console.WriteLine("{0}({1}) {2}", ts, lvl.ToString(), msg);
        }

        public override string Description { get { return "ConsoleLogger"; } }

        public override void Flush()
        {
        }

        public override void Close()
        {
            Console.WriteLine("Console log is closed.");
        }
    }
}
