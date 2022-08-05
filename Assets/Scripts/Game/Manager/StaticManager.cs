using System.IO;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticManager : IManager<StaticManager>
{
    private const string DATA_PATH = "Assets/Scripts/ScriptableObject/DataTable";
    private DBStaticLoader _StaticLoader = null;
    private Dictionary<string, IScriptableObject> _StaticDataTable = new Dictionary<string, IScriptableObject>();

    private void Start()
    {
        _StaticLoader = gameObject.AddComponent<DBStaticLoader>();
    }

    public void Init()
    {
        foreach (var file in Directory.GetFiles(DATA_PATH, "*.cs"))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            Type dataType = Type.GetType(fileName);
            IScriptableObject inst = ScriptableObject.CreateInstance(dataType) as IScriptableObject;
            _StaticDataTable.Add(fileName, inst);
        }
    }

    public void Load()
    {
        foreach(var item in _StaticDataTable.Values)
        {
            _StaticLoader.LoadTable(item);
        }
    }
}
