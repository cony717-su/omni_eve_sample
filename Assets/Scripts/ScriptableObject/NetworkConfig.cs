using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="NetworkConfig", menuName ="ScriptableObjects/NetworkConfig")]
public class NetworkConfig : IScriptableObject
{
    [SerializeField] private string _gameCode = "";
    public string GameCode => _gameCode;

    [SerializeField] private string _binaryVersion = "";
    public string BinaryVersion => _binaryVersion;

    [SerializeField] private string _cryptKey = "";
    public string CryptKey => _cryptKey;

    [SerializeField] private string _apiServer = "";
    public string ApiServer => _apiServer;
}

[CustomEditor(typeof(NetworkConfig))]
public class NetworkConfigEditor : Editor
{
    private NetworkConfig Script => target as NetworkConfig;

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