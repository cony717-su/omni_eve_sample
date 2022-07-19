using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using System.Text.RegularExpressions;

using MySql.Data;
using MySql.Data.MySqlClient;

using Shiftup.CommonLib.Data;
using Shiftup.CommonLib.Data.Attributes;
using Shiftup.CommonLib.Logger;

namespace InnerDevToolCommon.Database
{
    public static class DataReaderExtension
    {
        public static IEnumerable<IDictionary<string, Object>> DataRecord(this System.Data.IDataReader source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            while (source.Read())
            {
                IDictionary<string, Object> row = new Dictionary<string, Object>();

                for (int i = 0; i < source.FieldCount; i++)
                {
                    if (!row.ContainsKey(source.GetName(i)))
                    {
                        row.Add(new KeyValuePair<string, Object>(source.GetName(i), source.GetValue(i)));
                    }
                }

                yield return row;
            }
        }
    }

    public abstract class MysqlDatabase
    {
        protected GameDatabaseCache cache;
        protected MySqlConnection connection;
        protected DBConnectionInfo connectionInfo;
        protected string dbHostInfo;
        protected string dbName;

        public string Name
        {
            get
            {
                return this.dbName;
            }
        }

        public MysqlDatabase(DBConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
        }

        private string GetConnectionInfo()
        {
            StringBuilder connString = new StringBuilder();
            string[] connInfoHostNPort = this.dbHostInfo.Split(':');
            
            connString.AppendFormat("server = {0}; uid = {1}; pwd = {2}; database = {3}; pooling=false;",
                connInfoHostNPort[0],
                connectionInfo.DatabaseID,
                connectionInfo.DatabasePW,
                this.dbName);
            if (connInfoHostNPort.Length == 2 && !connInfoHostNPort[1].Equals("3306"))
            {
                connString.AppendFormat("port={0};", connInfoHostNPort[1]);
            }
            return connString.ToString();
        }

        private string GetConnectionInfoWithUtf8()
        {
            StringBuilder connString = new StringBuilder();
            string[] connInfoHostNPort = this.dbHostInfo.Split(':');
            connString.AppendFormat("server = {0}; uid = {1}; pwd = {2}; database = {3}; charset=utf8; pooling=false;",
                connInfoHostNPort[0],
                connectionInfo.DatabaseID,
                connectionInfo.DatabasePW,
                this.dbName);
            if (connInfoHostNPort.Length == 2 && !connInfoHostNPort[1].Equals("3306"))
            {
                connString.AppendFormat("port={0};", connInfoHostNPort[1]);
            }
            return connString.ToString();
        }

        public bool Connect()
        {
            string connectionInfo = GetConnectionInfoWithUtf8();
            try
            {

                connection = new MySqlConnection(connectionInfo);
                connection.Open();
            }
            catch
            {
                try
                {
                    connectionInfo = GetConnectionInfo();
                    connection = new MySqlConnection(connectionInfo);
                    connection.Open();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }


            if (!connection.State.Equals(ConnectionState.Open))
            {
                return false;
            }

            this.cache = new GameDatabaseCache();
            this.cache.Load(connectionInfo);

            return true;
        }

        public void Disconnect()
        {
            if (this.connection != null)
            {
                this.connection.Close();
            }

            this.connection = null;
        }

        public TableMeta GetTableMeta(string tableName)
        {
            var table = this.cache.GetTables().FirstOrDefault(meta => meta.Name == tableName);
            if (table != null)
            {
                return table;
            }

            return new TableMeta();
        }

        public bool IsExistTable(string tableName)
        {
            var tables = this.cache.GetTables();
            foreach (var table in tables)
            {
                if (table.Name == tableName)
                {
                    return true;
                }
            }

            return false;
        }

        public MySqlCommand QueryWithNoneReader(string query, IEnumerable<KeyValuePair<string, object>> sqlParameters)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            foreach (var parameter in sqlParameters)
            {
                parameters.Add(new MySqlParameter() { ParameterName = parameter.Key, Value = parameter.Value });
            }

            return QueryWithNoneReader(query, parameters);
        }

        public MySqlCommand QueryWithNoneReader(string query, params KeyValuePair<string, object>[] sqlParameters)
        {
            return QueryWithNoneReader(query, sqlParameters.ToList());
        }

        public MySqlCommand QueryWithNoneReader(string query, List<MySqlParameter> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Exception ie = e;
                while (ie.InnerException != null)
                {
                    System.Diagnostics.Debug.Print(ie.Message);
                    ie = ie.InnerException;
                }
                //MessageBox.Show(ie.ToString());
                throw;
            }

            return cmd;
        }

        public IEnumerable<IDictionary<string, object>> QueryWithReader(string query, IEnumerable<KeyValuePair<string, object>> sqlParameters)
        {
            List<MySqlParameter> listParameter = new List<MySqlParameter>();
            foreach (var parameter in sqlParameters)
            {
                listParameter.Add(new MySqlParameter() { ParameterName = parameter.Key, Value = parameter.Value });
            }

            return QueryWithReader(query, listParameter);
        }

        public IEnumerable<IDictionary<string, object>> QueryWithReader(string query, params KeyValuePair<string, object>[] sqlParameters)
        {
            List<MySqlParameter> listParameter = new List<MySqlParameter>();
            foreach (var parameter in sqlParameters)
            {
                listParameter.Add(new MySqlParameter() { ParameterName = parameter.Key, Value = parameter.Value });
            }

            return QueryWithReader(query, listParameter);
        }

        public IEnumerable<IDictionary<string, object>> QueryWithReader(string query, List<MySqlParameter> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }

            var reader = cmd.ExecuteReader();
            var objects = reader.DataRecord();
            foreach (var data in objects)
            {
                yield return data;
            }

            reader.Close();
        }

        public object QueryWithScalar(string query, params KeyValuePair<string, object>[] sqlParameters)
        {
            List<MySqlParameter> listParameter = new List<MySqlParameter>();
            foreach (var parameter in sqlParameters)
            {
                listParameter.Add(new MySqlParameter() { ParameterName = parameter.Key, Value = parameter.Value });
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);
            foreach (var parameter in listParameter)
            {
                cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }

            try
            {
                var result = cmd.ExecuteScalar();
                Console.WriteLine(result);
                return result;
            }
            catch (Exception e)
            {
                Log.Warning(e.Message);
            }

            return null;
        }

        public void TruncateTable(string tableName)
        {
            QueryWithNoneReader(String.Format("TRUNCATE TABLE {0}", tableName));
        }

        public void DeleteTable(string tableName, ulong nfguid)
        {
            string query = String.Format("DELETE FROM {0} WHERE nfguid = @nfguid", tableName);
            QueryWithNoneReader(query, new KeyValuePair<string, object>("@nfguid", nfguid));
        }
    }
}