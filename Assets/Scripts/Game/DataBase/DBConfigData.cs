using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using InnerDevToolCommon.Database;

public class DBConfigData
{
    public static IList<string> ConfigColumns = new List<string> { "dbProfileName", "mainDbHost", "gameDbHost", "mainDbName", "gameDbName", "IS_QA", "serverFolderName", "dbID" };
    public static IDictionary<string, DBConnectionInfo> DBConnDict = new Dictionary<string, DBConnectionInfo>();

    public static bool CheckConfigFile { get; set; }

    public static string ConfigFileFullPath { get; set; }

    public static string CONFIG_FILE_NAME { get { return "\\config"; } }
    public static string DEFAULT_CONFIG_DB_CONN_PROFILE { get { return "default(dc_dev)"; } }

    public static void ConfigFileLoad()
    {
        string configFilePath = ConfigFileFullPath;
        DBConnDict.Clear();

        using (FileStream fs = new FileStream(configFilePath, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
            {
                string strLineValue = null;
                string[] values = null;

                while ((strLineValue = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(strLineValue)) return;

                    if (strLineValue.Substring(0, 1).Equals("#"))
                        continue;

                    values = strLineValue.Split(',');
                    if (values.Length < DBConfigData.ConfigColumns.Count)
                        return;

                    DBConnectionInfo dbConnInfo = new DBConnectionInfo();
                    dbConnInfo.DatabaseProfileName = values[0];
                    dbConnInfo.MainDatabaseHostInfo = values[1];
                    dbConnInfo.GameDatabaseHostInfo = values[2];

                    dbConnInfo.MainDatabaseName = values[3];
                    dbConnInfo.GameDatabaseName = values[4];
                    dbConnInfo.IsQA = values[5];
                    dbConnInfo.ServerUrl = values[6];
                    dbConnInfo.DatabaseID = values[7];
                    if (dbConnInfo.DatabaseID == "shiftup")
                    {
                        dbConnInfo.DatabasePW = "f1234";
                    }
                    else if (dbConnInfo.DatabaseID == "dc")
                    {
                        dbConnInfo.DatabasePW = "dc1234";
                    }
                    if (values.Length > 8)
                    {
                        dbConnInfo.DatabasePW = values[8];
                    }

                    DBConnDict.Add(dbConnInfo.DatabaseProfileName, dbConnInfo);
                }
            }
        }
    }

    public static IDictionary<string, DBConnectionInfo> GetConfigData()
    {
        return DBConnDict;
    }

    public static IDictionary<string, DBConnectionInfo> SetDefaultConnData()
    {
        DBConnectionInfo connInfo = new DBConnectionInfo();
        connInfo.DatabaseProfileName = DEFAULT_CONFIG_DB_CONN_PROFILE;
        connInfo.MainDatabaseHostInfo = "kr.dc.dev.shiftup.co.kr:3306";
        connInfo.GameDatabaseHostInfo = "kr.dc.dev.shiftup.co.kr:3306";
        connInfo.MainDatabaseName = "dc_dev";
        connInfo.GameDatabaseName = "dc_dev_game";
        connInfo.DatabaseID = "shiftup";
        connInfo.DatabasePW = "f1234";
        connInfo.IsQA = "0";
        connInfo.ServerUrl = "kr.dc.dev.shiftup.co.kr/dev";

        DBConnDict.Add(connInfo.DatabaseProfileName, connInfo);

        return DBConnDict;
    }
}