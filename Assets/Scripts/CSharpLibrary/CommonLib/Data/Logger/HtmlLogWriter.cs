using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Shiftup.CommonLib.Logger
{
    public class HtmlLogWriter : LogWriter
    {
        private readonly StreamWriter file = null;
        private readonly string filename;
        static private readonly string htmlHead = @"
<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">
<html>
    <head>
         <meta charset=""UTF-8"">
        <script>
            function toggle(tag)
            {
                var elements = document.getElementsByClassName(tag);
                for( var i = 0 ; i < elements.length ; i++) {
                    elements[i].classList.toggle('hidden');
                }
            }
        </script>
        <style type=""text/css"">
            .hidden { visibility: collapse; }
            table.log th {
    	        background-color: #999999;
	            color: #ffffff;
            }
            table.log tr.Debug {
    	        background-color: #ffffff;
	            color: #666666;
            }
            table.log tr.Info {
    	        background-color: #ffffff;
	            color: #666666;
            }
            table.log tr.Warning {
    	        background-color: #ffff00;
	            color: #000000;
            }
            table.log tr.Error {
    	        background-color: #ff0000;
	            color: #ffffff;
            }
        </style>
    </head>
    <body>
        <input type=""checkbox"" id=""chError"" checked=""checked"" onclick=""toggle('Error')""/>
        <label for=""chError"">Error</label>
        <input type=""checkbox"" id=""chWarning"" checked=""checked"" onclick=""toggle('Warning')""/>
        <label for=""chWarning"">Warning</label>
        <input type=""checkbox"" id=""chInfo"" checked=""checked"" onclick=""toggle('Info')""/>
        <label for=""chInfo"">Info</label>
        <input type=""checkbox"" id=""chDebug"" checked=""checked"" onclick=""toggle('Debug')""/>
        <label for=""chDebug"">Debug</label>
        <table class=""log"">
            <tr><th>Time</th><th>Level</th><th>Message</th></tr>";

        static private readonly string htmlTail = @"
        </table>
    </body>
</html>";

        public HtmlLogWriter(string filename, Levels lvl)
            : base(Types.HtmlFile, lvl)
        {
            this.file = new StreamWriter(filename);
            this.filename = filename;

            this.file.WriteLine(htmlHead);
        }

        public override string Description { get { return String.Format("HtmlLogger {0}", this.filename); } }

        public override void AddLine(string ts, Levels lvl, string msg)
        {
            //file.WriteLine()
            file.WriteLine(String.Format("<tr class=\"{1}\"><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ts, lvl.ToString(), msg));
        }

        public override void Flush()
        {
            file.Flush();
        }

        public override void Close()
        {
            if (file == null)
                return;

            file.WriteLine(htmlTail);
            file.Flush();
            file.Close();
        }
    }
}
