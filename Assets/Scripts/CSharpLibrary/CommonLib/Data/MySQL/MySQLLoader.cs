using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Common;

using MySql.Data;
using MySql.Data.MySqlClient;

using Shiftup.CommonLib.Logger;

namespace Shiftup.CommonLib.Data.MySQL
{
    public class MySQLLoader : DBLoaderWithSQL
    {
        private MySqlConnection connection = null;

        public MySQLLoader()
        {
            this.metaTable = "information_schema.columns";
            this.metaFields = new string[] { "TABLE_NAME", "COLUMN_NAME", "DATA_TYPE" };
            this.metaWhere = string.Empty;
            typeNameLookup = (new Dictionary<string, TypeCode>() {
                { "char", TypeCode.String },
                { "varchar", TypeCode.String },
                { "bigint", TypeCode.UInt64 },
                { "smallint", TypeCode.Int16 },
                { "int", TypeCode.Int32 },
                { "tinyint", TypeCode.Boolean },
                { "float", TypeCode.Double },
                { "text", TypeCode.String },
                { "datetime", TypeCode.DateTime },
                { "timestamp", TypeCode.DateTime },
                { "tinytext", TypeCode.String },
                { "mediumtext", TypeCode.String },
            }).AsReadOnly();
        }
        public override void PreLoad() { }
        public override void PostLoad() { }
        public override string BuildEntityName(string name) { return String.Format("`{0}`", name); }

        private const string failMessage = "데이터베이스 접속 실패 : {0}";
        public override string Init(string connectionString)
        {
            var info = MySQLConnectionInfo.Decrypt(connectionString);

            try
            {
                connection = new MySqlConnection(info.ToString());
                connection.Open();
            }
            catch (Exception e)
            {
                throw new MessageException(e, failMessage, info.GetInfo());
            }
            if (connection.State != ConnectionState.Open)
                throw new MessageException(failMessage, info.GetInfo());

            this.metaWhere = String.Format("TABLE_SCHEMA = '{0}'", connection.Database);
            return info.GetInfo();
        }

        public override void Close()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();

            connection = null;
        }

        public IEnumerable<IDictionary<string, Object>> LoadTableWithAllColumn(string tableName)
        {
            var reader = new DBRowReader(getCommand(String.Format("SELECT * FROM {0}", tableName)).ExecuteReader());

            foreach (var row in reader.Rows())
                yield return row.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            reader.Close();
        }

        protected override DbCommand getCommand(string query)
        {
            return new MySqlCommand(query, connection);
        }


    }
}

