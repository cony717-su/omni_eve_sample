using System;
using System.IO;
using UnityEngine;

public class IScriptableObject : ScriptableObject
{
    private string _FileName;

    private string FileName
    {
        get
        {
            if (string.IsNullOrEmpty(_FileName))
            {
                return GetType().Name;
            }

            return _FileName;
        }
    }

    private string GetFullName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = FileName;
        }
        string userConfigDir = Application.persistentDataPath + "\\";
        return userConfigDir + fileName + ".json";
    }

    public static T Create<T>(string fileName = "") where T : IScriptableObject
    {
        T inst = ScriptableObject.CreateInstance(typeof(T)) as T;
        inst._FileName = fileName;
        inst.Load(fileName);
        return inst;
    }

    public void Load(string fileName = "")
    {
        string fullName = GetFullName(fileName);
        if (!File.Exists(fullName))
        {
            DebugManager.Log($"!File.Exists {fullName}");
            return;
        }
        var json = File.ReadAllText(fullName);
        JsonUtility.FromJsonOverwrite(json, this);
    }
    
    public void Save(string fileName = "")
    {
        string fullName = GetFullName(fileName);
        if (!File.Exists(fullName))
        {
            DebugManager.Log($"!File.Exists {fullName}");
            return;
        }
        var json = JsonUtility.ToJson(this, true);
        File.WriteAllText(fullName, json);
    }
}