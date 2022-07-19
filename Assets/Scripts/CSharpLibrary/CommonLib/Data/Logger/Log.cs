using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Shiftup.CommonLib.Logger
{
    public enum Types
    {
        DebugOutput,
        Console,
        Form,
        File,
        HtmlFile
    }

    public enum Levels
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
    }
    public static class Log
    {
        [Flags]
        public enum LogTypes
        {
            TextFile = 0x1,
            HtmlFile = 0x2,
            Console = 0x4,

        }
        public class LogCounts
        {
            public int this[Levels idx] { get { return _dict[idx]; } }

            private Dictionary<Levels, int> _dict = new Dictionary<Levels, int>();

            public LogCounts(Dictionary<Levels, int> dict) { _dict = dict; }
        }

        public static IEnumerable<string> LogFiles
        {
            get { return _logFiles; }
        }

        public static string LogFolder = "log";
        public static bool LogWithThreadId = false;
        public static Levels DefaultLogLevel = Levels.Info;
        public static readonly LogCounts Counts = new LogCounts(logCountDict);

        public static int FlushIntervalSeconds = 0;

        private static Levels minLevel = Levels.Debug;

        public static void Debug(string format, params object[] args)
        {
            Write(Levels.Debug, format, args);
        }

        public static void Info(string format, params object[] args)
        {
            Write(Levels.Info, format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            Write(Levels.Warning, format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Write(Levels.Error, format, args);
        }

        public static void ErrorOnCheckFile(string fn)
        {
            Write(Levels.Error, "파일({0})을 찾을수 없습니다.", fn);
        }

        public static void ErrorOnLoadFile(Exception e, string fn)
        {
            ErrorWithException(e, "파일({0})을 로드할 수 없습니다.", fn);
        }

        public static void ErrorWithException(Exception e, string msg, params object[] args)
        {
            if (Write(Levels.Error, msg, args) == false)
                return;

            while (e != null)
            {
                Write(Levels.Warning, "{0} - {1}", e.Message, e.Source);
                string[] lines = e.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string s in lines)
                    Write(Levels.Warning, s);
                e = e.InnerException;
            }
        }

        [Obsolete]
        public static void Write(Exception e)
        {
            OnException(Levels.Error, e);
        }

        [Obsolete]
        public static void OnException(Exception e)
        {
            OnException(Levels.Error, e);
        }

        public static void OnException(Levels lvl, Exception e)
        {
            while (e != null)
            {
                if (Write(lvl, "{0} - {1}", e.Message, e.Source) == false)
                    return;
                string[] lines = e.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string s in lines)
                    Write(lvl, s);
                e = e.InnerException;
            }
        }

        public static void Summary()
        {
            Log.Info("로그 시스템 정보");
            foreach (var writer in writers)
                Log.Info("{0}, 로그 수준({1}) ", writer.Description, writer.Level.ToString());
        }


        [Obsolete]
        public static void Write(string format, params object[] args)
        {
            Write(DefaultLogLevel, format, args);
        }

        public static bool Write(Levels lvl, string format, params object[] args)
        {
            if (closed || lvl < minLevel)
                return false;

            lock (_lock)
            {
                var now = DateTime.Now;


                string ts = now.ToString("yyyy-MM-dd hh:mm:ss.fff");
                string header;

                if (LogWithThreadId)
                    header = String.Format("{0}[{1}]", ts, System.Threading.Thread.CurrentThread.ManagedThreadId);
                else
                    header = ts;

                string message = String.Format(format, args);

                logCountDict[lvl]++;

                bool flushNow = false;
                if ((new TimeSpan(now.Ticks - lastFlushTick)).TotalSeconds > FlushIntervalSeconds
                        && FlushIntervalSeconds > 0)
                {
                    flushNow = true;
                    lastFlushTick = now.Ticks;
                }

                sendToWriter(header, lvl, message, flushNow);
            }
            return true;
        }

        public static void AddConsoleLogger()
        {
            AddConsoleLogger(DefaultLogLevel);
        }

        public static void AddConsoleLogger(Levels lvl)
        {
            if (writers.Exists(l => { return l.LoggerType == Types.Console; }) == false)
                addLogger(new ConsoleLogger(lvl));
        }

        public static string AddFileLogger()
        {
            return AddFileLogger(DefaultLogLevel);
        }

        public static string AddFileLogger(Levels lvl)
        {
            return AddFileLogger(DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt", lvl);
        }

        public static string AddFileLogger(string filename)
        {
            return AddFileLogger(filename, DefaultLogLevel);
        }

        public static string AddFileLogger(string filename, Levels lvl)
        {
            if (Directory.Exists(LogFolder) == false)
                Directory.CreateDirectory(LogFolder);

            string path = Path.Combine(LogFolder, filename);
            addLogger(new FileLogger(path, lvl));

            _logFiles.Add(path);
            return path;
        }


        public static string AddHtmlLogger()
        {
            return AddHtmlLogger(DefaultLogLevel);
        }
        public static string AddHtmlLogger(Levels lvl)
        {
            return AddHtmlLogWriter(DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html", lvl);
        }

        public static string AddHtmlLogWriter(string filename)
        {
            return AddHtmlLogWriter(filename, DefaultLogLevel);
        }
        public static string AddHtmlLogWriter(string filename, Levels lvl)
        {
            if (Directory.Exists(LogFolder) == false)
                Directory.CreateDirectory(LogFolder);

            string path = Path.Combine(LogFolder, filename);
            addLogger(new HtmlLogWriter(path, lvl));

            _logFiles.Add(path);
            return path;
        }

        public static void AddDebugOutputLogger()
        {
            AddDebugOutputLogger(DefaultLogLevel);
        }
        public static void AddDebugOutputLogger(Levels lvl)
        {
            if (writers.Exists(l => { return l.LoggerType == Types.DebugOutput; }) == false)
                addLogger(new DebugOutputLogger(lvl));
        }

        public static void RemoveLogger(LogWriter logger)
        {
            writers.Remove(logger);
            updateMinLevel();
        }

        public static void ChangeLoggerLevel(Types t, Levels l)
        {
            foreach (var writer in writers)
            {
                if (writer.LoggerType == t)
                    writer.Level = l;
            }
            updateMinLevel();
        }

        public static void CloseLogWriters()
        {
            closed = true;
            foreach (var writer in writers)
                writer.Close();

            writers.Clear();
            minLevel = Levels.Debug;
        }

        private static long lastFlushTick = 0;

        private static bool closed = false;
        private static object _lock = new object();
        private static List<string> _logFiles = new List<string>();
        private static Dictionary<Levels, int> logCountDict = new Dictionary<Levels, int>();

        private static List<LogWriter> writers = new List<LogWriter>();

        static Log()
        {
            lastFlushTick = DateTime.Now.Ticks;
            foreach (Levels lvl in Enum.GetValues(typeof(Levels)))
                logCountDict.Add(lvl, 0);

            updateMinLevel();
        }

        private static void addLogger(LogWriter writer)
        {
            writers.Add(writer);
            updateMinLevel();
        }

        private static void sendToWriter(string header, Levels lvl, string msg, bool flushNow)
        {
            foreach (var writer in writers)
            {
                if (lvl >= writer.Level)
                    writer.AddLine(header, lvl, msg);

                if (flushNow)
                    writer.Flush();
            }
        }

        private static void updateMinLevel()
        {
            if (writers.Any())
                minLevel = writers.Select(writer => writer.Level).Min();
        }
    }
}
