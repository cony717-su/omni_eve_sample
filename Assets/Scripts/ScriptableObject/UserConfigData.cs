
using System.Diagnostics;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName ="UserConfigData", menuName ="ScriptableObjects/UserConfig", order = 1)]
public class UserConfigData : IScriptableObject
{
    public string serverName;
    public string nickName;
    public string bin;
    public string guestID;
}

[CustomEditor(typeof(UserConfigData))]
public class UserConfigDataEditor : Editor
{
    [SerializeField]
    public UserConfigData Script => target as UserConfigData;

    private void OnEnable()
    {
        Script.Load();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Load UserConfig"))
        {
            Script.Load();
        }

        if (GUILayout.Button("Save UserConfig"))
        {
            Script.Save();
        }

        if (GUILayout.Button("Open Folder"))
        {
            if (!Directory.Exists(Script.FilePath))
                return;

            Application.OpenURL("file://" + Script.FilePath);
        }
    }
}