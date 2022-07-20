
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
}

[CustomEditor(typeof(UserConfigData))]
public class UserConfigDataEditor : Editor
{
    [SerializeField]
    public UserConfigData _UserConfigData;

    private void OnEnable()
    {
        _UserConfigData = (UserConfigData)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Load UserConfig"))
        {
            _UserConfigData.Load();
        }
        
        if (GUILayout.Button("Save UserConfig"))
        {
            _UserConfigData.Save();
        }
    }
}