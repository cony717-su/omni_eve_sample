using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


using InnerDevTool;
using InnerDevTool.Data;
using InnerDevToolCommon;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Database;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

public class DBStaticLoader : MonoBehaviour
{
    public UserConfigData _UserConfigData;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        LoadConfigFile();

        _UserConfigData = UserConfigData.Create<UserConfigData>();

        Connect(GetNickName(), GetServerName());
    }

    public void LoadTable(IScriptableObject dataTable)
    {
        // todo
        /*StaticOmniEveFloor mapData = DBLookup.Instance.StaticData.StaticOmniEveFloorTable.Get(floorIndex);
        Debug.Log($"Load MapData: {mapData.width}, {mapData.height}");*/

        var property = dataTable.GetType().GetProperty("_DataTable");
        property.SetValue(dataTable, Activator.CreateInstance(property.PropertyType));

        Type staticRowType = property.PropertyType.GetGenericArguments()[0];
        var metas = RowData.GetMetas(staticRowType);
        var dataObjects = DatabaseApplier.Instance.SelectRows(staticRowType, metas);
        var table = property.GetValue(dataTable) as IRowStorage;
        foreach (var data in dataObjects)
        {
            table.BuildRow(data);
        }
    }

    private void OnDestroy()
    {
        if (DBLookup.Instance.Main != null)
        {
            DBLookup.Instance.Main.Disconnect();
        }

        if (DBLookup.Instance.Game != null)
        {
            DBLookup.Instance.Game.Disconnect();
        }

        DebugManager.Log("Disconnect DB");
    }

    bool Connect(string nickName, string dbName)
    {
        if (string.IsNullOrEmpty(nickName) || string.IsNullOrEmpty(dbName))
        {
            DebugManager.Log($"IsNullOrEmpty Connect: {dbName} {nickName}");
            return false;
        }
        
        if(!ConnectDB(dbName))
        {
            return false;
        }

        var nfguid = DBLookup.Instance.Main.GetNfguid(nickName);
        if (nfguid == 0)
        {
            DebugManager.Log($"invalid nickName: {nickName}");
            return false;
        }

        DebugManager.Log($"UserName: {nickName} ({nfguid})");
        DBLookup.Instance.StaticData = new StaticData();
        DBLookup.Instance.StaticData.Init();

        // todo
        StaticManager.Instance.Load();
        return true;
    }

    bool ConnectDB(string dbName)
    {
        var config = DBConfigData.DBConnDict[dbName];
        if (config is null)
        {
            DebugManager.Log($"invalid serverName: {dbName}");
            return false;
        }
        
        if (DBLookup.Instance.Main != null)
        {
            DBLookup.Instance.Main.Disconnect();
        }

        DBLookup.Instance.Main = new MainSql(config);
        if (!DBLookup.Instance.Main.Connect())
        {
            return false;
        }
        DebugManager.Log($"Connect DB Main: {dbName}");

        if (DBLookup.Instance.Game != null)
        {
            DBLookup.Instance.Game.Disconnect();
        }

        DBLookup.Instance.Game = new GameSql(config);
        if (!DBLookup.Instance.Game.Connect())
        {
            return false;
        }

        DatabaseCollection.Clear();
        DatabaseCollection.Register(DBLookup.Instance.Main);
        DatabaseCollection.Register(DBLookup.Instance.Game);

        DebugManager.Log($"Connect DB Game: {dbName}");
        return true;
    }

    string GetNickName()
    {
        string nickName = _UserConfigData.nickName;
        if (string.IsNullOrEmpty(nickName))
        {
            return GameManager.Instance.nickName;
        }

        return nickName;
    }
    string GetServerName()
    {
        string serverName = _UserConfigData.serverName;
        if (string.IsNullOrEmpty(serverName))
        {
            return GameManager.Instance.serverName;
        }

        return serverName;
    }

    void LoadConfigFile()
    {
        string configDir = Application.dataPath + "\\config";
        string configFile = DBConfigData.CONFIG_FILE_NAME + ".csv";
        DirectoryInfo chkConfigDir = new DirectoryInfo(configDir);
        if (chkConfigDir.Exists == false)
        {
            chkConfigDir.Create();
        }

        FileInfo configFileInfo = new FileInfo(configDir + configFile);
        DBConfigData.ConfigFileFullPath = configDir + configFile;
        if (configFileInfo.Exists == false)
        {
            DebugManager.Log("Load Default Config Data");
            DBConfigData.SetDefaultConnData();
            DBConfigData.CheckConfigFile = false;
        }
        else
        {
            DBConfigData.CheckConfigFile = true;
        }

        DBConfigData.ConfigFileLoad();
    }
}