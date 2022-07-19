using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

namespace Shiftup.CommonLib.Data
{
    using Row = IEnumerable<KeyValuePair<string, Object>>;

    class DBRowReader : IRowReader
    {
        public bool IsValid { get { return reader != null; } }
        private IDataReader reader;
        public DBRowReader(IDataReader dataReader)
        {
            this.reader = dataReader;
        }
        public IEnumerable<Row> Rows()
        {
            if (reader == null)
                yield break;

            while (reader.Read())
                yield return buildRow();
        }
        public void Close()
        {
            this.reader.Close();
            this.reader = null;
        }

        private IEnumerable<KeyValuePair<string, Object>> buildRow()
        {
            for (int i = 0; i < reader.FieldCount; i++)
                yield return new KeyValuePair<string, Object>(reader.GetName(i), reader.GetValue(i));
        }
    }
}
