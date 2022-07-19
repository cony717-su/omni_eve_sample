using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;

using Shiftup.CommonLib;
using Shiftup.CommonLib.Data;
using Shiftup.CommonLib.Data.MySQL;

namespace InnerDevToolCommon.Database
{
    public class GameDBLoader : DBLoaderWithSQL
    {
        private MySqlConnection connection = null;

        public GameDBLoader()
        {
            this.metaTable = "information_schema.columns";
            this.metaFields = new string[] { "TABLE_NAME", "COLUMN_NAME", "DATA_TYPE" };
            this.metaWhere = string.Empty;
            this.typeNameLookup = (new Dictionary<string, TypeCode>()
            {
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
                { "longtext", TypeCode.String }
            }).AsReadOnly();
        }

        protected override DbCommand getCommand(string query)
        {
            return new MySqlCommand(query, connection);
        }

        public override string BuildEntityName(string name)
        {
            return String.Format("`{0}`", name);
        }

        public override void Close()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();

            connection = null;
        }

        public override string Init(string connectionString)
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception e)
            {
                throw new MessageException(e, "Fail to {0}", connectionString);
            }
            if (connection.State != ConnectionState.Open)
            {
                throw new MessageException("Fail to {0}", connectionString);
            }
            this.metaWhere = String.Format("TABLE_SCHEMA = '{0}'", connection.Database);

            return connectionString;
        }

        public override void PostLoad()
        {
        }

        public override void PreLoad()
        {
        }
    }
}