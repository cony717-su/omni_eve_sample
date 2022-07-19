using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Dynamic;

using Shiftup.CommonLib.Logger;
using Microsoft.VisualBasic.FileIO;

namespace Shiftup.CommonLib
{
    internal class Column
    {
        public string name;

        private Type parentType;
        private Member member;

        private abstract class Member
        {
            public abstract Type MemberType { get; }
            public abstract object GetValue(object obj);
            public abstract void SetValue(object obj, object value);
            static public Member Create(Type t, string name_)
            {
                var pi = t.GetProperty(name_);
                if (pi != null)
                    return new PropertyMember(pi);

                var fi = t.GetField(name_);
                if (fi != null)
                    return new FieldMember(fi);

                return null;
            }
        }

        private class PropertyMember : Member
        {
            private PropertyInfo pi;
            public override Type MemberType { get { return pi.PropertyType; } }
            public PropertyMember(PropertyInfo i)
            {
                pi = i;
            }

            public override object GetValue(object obj)
            {
                return pi.GetValue(obj, null);
            }
            public override void SetValue(object obj, object value)
            {
                pi.SetValue(obj, value, null);
            }
        }

        private class FieldMember : Member
        {
            private FieldInfo fi;
            public override Type MemberType { get { return fi.FieldType; } }
            public FieldMember(FieldInfo i)
            {
                fi = i;
            }

            public override object GetValue(object obj)
            {
                return fi.GetValue(obj);
            }

            public override void SetValue(object obj, object value)
            {
                fi.SetValue(obj, value);
            }
        }

        public Column(Type t, string name_)
        {
            parentType = t;
            name = name_;
            member = Member.Create(t, name_);
        }

        public string GetValueString(object obj, bool useQuote, bool ignoreNewline)
        {
            var value = member.GetValue(obj);
            string valueString;

            if (value == null)
                valueString = String.Empty;
            else
                valueString = value.ToString();

            if (Type.GetTypeCode(member.MemberType) == TypeCode.String && useQuote)
            {
                string txt = valueString.Replace("\"", "\"\"");
                string ret = String.Format("\"{0}\"", txt);
                if (ignoreNewline)
                    ret = ret.Replace("\r", "").Replace("\n", "");
                return ret;
            }
            else
            {
                return valueString;
            }
        }

        public object GetValue(object obj)
        {
            return member.GetValue(obj);
        }

        public void SetValue(object obj, string v)
        {
            member.SetValue(obj, v.ToConvert(Type.GetTypeCode(member.MemberType)));
        }
    }

    static public class CSVExport
    {
        static public string ToCSVExport(this string str)
        {
            return String.Format("\"{0}\"", str.Replace("\"", "\"\""));
        }
    }

    public class CSVFile
    {
        public static List<string[]> LoadFile(string filename, string delimiter, bool includeComment = false, bool quote = true)
        {
            List<string[]> ret = new List<string[]>();
            try
            {
                var parser = new TextFieldParser(filename);
                parser.SetDelimiters(delimiter);
                if (includeComment == false)
                    parser.CommentTokens = new string[] { "//" };
                parser.TextFieldType = FieldType.Delimited;
                parser.HasFieldsEnclosedInQuotes = quote;

                while (parser.EndOfData == false)
                    ret.Add(parser.ReadFields());

                parser.Close();
            }
            catch (Exception e)
            {
                Log.ErrorOnLoadFile(e, filename);
            }
            return ret;
        }

        public static List<string[]> LoadFile(Stream stream, string delimiter, Encoding encoding)
        {
            List<string[]> ret = new List<string[]>();

            var parser = new TextFieldParser(stream, encoding);
            parser.SetDelimiters(delimiter);
            parser.CommentTokens = new string[] { "//" };
            parser.TextFieldType = FieldType.Delimited;
            parser.HasFieldsEnclosedInQuotes = true;

            while (parser.EndOfData == false)
                ret.Add(parser.ReadFields());

            parser.Close();
            return ret;
        }

        public static List<string[]> LoadFile(string filename, string delimiter, Encoding encoding)
        {
            List<string[]> ret = new List<string[]>();

            var parser = new TextFieldParser(filename, encoding);
            parser.SetDelimiters(delimiter);
            parser.CommentTokens = new string[] { "//" };
            parser.TextFieldType = FieldType.Delimited;
            parser.HasFieldsEnclosedInQuotes = true;

            while (parser.EndOfData == false)
                ret.Add(parser.ReadFields());

            parser.Close();
            return ret;
        }
        public static void SaveFile(string filename, string[] header, IEnumerable<string[]> data, Encoding encoding, bool commentHeader, char splitCharacter)
        {
            string splitter = new string(splitCharacter, 1);
            StreamWriter sw = new StreamWriter(filename, false, encoding);

            if (header != null)
            {
                if (commentHeader)
                    sw.WriteLine("// " + string.Join(splitter, header));
                else
                    sw.WriteLine(string.Join(splitter, header));
            }

            foreach (string[] line in data)
            {
                sw.WriteLine(String.Join(splitter, line));
            }

            sw.Flush();
            sw.Close();
        }

        private Type dataType;
        private Dictionary<int, Column> columnDict = null;
        private Column[] columns = null;
        private readonly string delimiter;
        private bool useQuoteCharacter = false;
        private bool ignoreNewline = false;

        public CSVFile(Type t, string sp)
        {
            dataType = t;
            delimiter = sp;
        }

        public void UseQuoteCharacter()
        {
            useQuoteCharacter = true;
        }

        public void IgnoreNewLine()
        {
            ignoreNewline = true;
        }

        public void AutoColumns()
        {
            List<string> names = new List<string>();
            foreach (var pi in dataType.GetProperties())
                names.Add(pi.Name);

            foreach (var fi in dataType.GetFields())
                names.Add(fi.Name);

            SetColumns(names.ToArray());
        }
        public void SetColumns()
        {
            FieldInfo field = dataType.GetField("Columns");

            var v = field.GetValue(null);

            if (v.GetType() == typeof(string[]))
                SetColumns((string[])v);
        }

        public void SetColumns(string[] names)
        {
            columns = new Column[names.Length];

            for (int i = 0; i < columns.Length; i++)
                columns[i] = new Column(dataType, names[i]);
        }

        public IEnumerable<T> LoadFileWithHeader<T>(string filename, bool skipNotExistField = false)
        {
            return LoadFileWithHeader(filename).Cast<T>();
        }
        public IEnumerable<object> LoadFileWithHeader(string filename, bool skipNotExistField = false)
        {
            List<string[]> lines = CSVFile.LoadFile(filename, this.delimiter, true);

            // header 
            var header = lines.First();
            lines.RemoveAt(0);

            if (header[0].StartsWith("// ") == true)
                header[0] = header[0].Substring(3);

            AutoColumns();
            columnDict = new Dictionary<int, Column>();
            for (int i = 0; i < header.Length; i++)
            {
                var col = columns.Where(c => c.name.CompareTo(header[i]) == 0);
                if (col.Any() == true)
                {
                    columnDict.Add(i, col.First());
                }
                else
                {
                    if (skipNotExistField == false)
                    {
                        throw new MessageException("CSV파일({0})에 필드({1})가 없습니다.", filename, header[i]);
                    }
                }
            }

            foreach (var line in lines)
                yield return buildObjectWithDict(line);
        }

        public IEnumerable<T> LoadFile<T>(string filename, bool quote = true)
        {
            List<string[]> lines = CSVFile.LoadFile(filename, this.delimiter, false, quote);

            foreach (var line in lines)
                yield return buildObject<T>(line);
        }

        public Dictionary<K, T> LoadFileWithKey<K, T>(string filename, string keyName)
        {
            var list = LoadFile<T>(filename);
            Dictionary<K, T> ret = new Dictionary<K, T>();

            Column keyColumn = null;

            foreach (Column c in columns)
            {
                if (c.name.CompareTo(keyName) == 0)
                {
                    keyColumn = c;
                    break;
                }
            }

            if (keyColumn == null)
                return null;

            foreach (T entry in list)
            {
                K key = (K)keyColumn.GetValue(entry);
                ret.Add(key, entry);
            }

            return ret;
        }

        public IDictionary<object, T> LoadFileWithKey<T>(string filename, string key)
        {
            var list = LoadFile<T>(filename);
            IDictionary<object, T> ret = new Dictionary<object, T>();

            Column keyColumn = null;

            foreach (Column c in columns)
            {
                if (c.name.CompareTo(key) == 0)
                {
                    keyColumn = c;
                    break;
                }
            }

            if (keyColumn == null)
                return null;

            foreach (T entry in list)
            {
                ret.Add(keyColumn.GetValue(entry), entry);
            }

            return ret;
        }
        public void SaveFile<T>(string filename, IEnumerable<T> data, bool writeColumnName, System.Text.Encoding encoding)
        {
            saveFile(new StreamWriter(filename, false, encoding), data, writeColumnName);
        }
        public void SaveFile<T>(string filename, IEnumerable<T> data, bool writeColumnName)
        {
            saveFile(new StreamWriter(filename), data, writeColumnName);
        }

        protected void saveFile<T>(StreamWriter sw, IEnumerable<T> data, bool writeColumnName)
        {
            if (writeColumnName)
            {
                string[] line = columns.Select(c => c.name).ToArray();

                sw.WriteLine("// " + String.Join(delimiter, line));

            }
            foreach (T line in data)
            {
                string[] cols = buildText(line);
                sw.WriteLine(String.Join(delimiter, cols));
            }

            sw.Flush();
            sw.Close();
        }
        private object buildObjectWithDict(string[] values)
        {
            object ret = Activator.CreateInstance(dataType);
            for (int i = 0; i < values.Length; i++)
            {
                if (columnDict.ContainsKey(i) == false)
                    continue;

                columnDict[i].SetValue(ret, values[i]);
            }

            return ret;
        }
        private T buildObject<T>(string[] values)
        {
            T ret = (T)Activator.CreateInstance(typeof(T));
            for (int i = 0; i < values.Length && i < columns.Length; i++)
                columns[i].SetValue(ret, values[i]);

            return ret;
        }

        private string[] buildText(object value)
        {
            string[] ret = new string[columns.Length];

            for (int i = 0; i < columns.Length; i++)
                ret[i] = columns[i].GetValueString(value, useQuoteCharacter, ignoreNewline);

            return ret;
        }
    }

    public class CSVFile<T> : CSVFile
    {
        public CSVFile(string delimiter) : base(typeof(T), delimiter) { }

        public IEnumerable<T> LoadFile(string filename, bool quote = true)
        {
            return base.LoadFile<T>(filename, quote);
        }
        public new Dictionary<K, T> LoadFileWithKey<K>(string filename, string keyName)
        {
            return base.LoadFileWithKey<K, T>(filename, keyName);
        }
        public IDictionary<object, T> LoadFileWithKey(string filename, string key)
        {
            return base.LoadFileWithKey<T>(filename, key);
        }
        public void SaveFile(string filename, IEnumerable<T> data, bool writeColumnName, System.Text.Encoding encoding)
        {
            saveFile(new StreamWriter(filename, false, encoding), data, writeColumnName);
        }
        public void SaveFile(string filename, IEnumerable<T> data, bool writeColumnName)
        {
            saveFile(new StreamWriter(filename), data, writeColumnName);
        }

        private void saveFile(StreamWriter sw, IEnumerable<T> data, bool writeColumnName)
        {
            base.saveFile<T>(sw, data, writeColumnName);
        }
    }
}
