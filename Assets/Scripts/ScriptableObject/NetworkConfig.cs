using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Network = Microsoft.VisualBasic.Devices.Network;

[CreateAssetMenu(fileName ="NetworkConfig", menuName ="ScriptableObjects/NetworkConfig")]
public class NetworkConfig : IScriptableObject
{
    [SerializeField] private string _gameCode = "";
    public string GameCode => _gameCode;

    [SerializeField] private string _binaryVersion = "";
    public string BinaryVersion => _binaryVersion;
    
    [SerializeField] private string _clientVersion = "";
    public string ClientVersion => _clientVersion;

    [SerializeField] private string _cryptKey = "";
    public string CryptKey => _cryptKey;

    [SerializeField] private string _apiServer = "";
    public string ApiServer => _apiServer;
    
    [SerializeField] private string _gameServer = "";
    public string GameServer => _gameServer;
    
    [SerializeField] private string _os = "";
    public string OS => _os;

    // todo
    [SerializeField] private string _action = "";
    public string Action => _action;
    
    // todo
    [SerializeField] private string _nickName = "";
    public string NickName => _nickName;
    
    // todo
    [SerializeField] private string _nfGuid = "";
    public string NfGuid => _nfGuid;
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


        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Post Test"))
        {
            NetworkManager.Instance.Init();
            NetworkManager.Instance.DoLogicDataOph();
        }
        
        GUILayout.EndHorizontal();
    }
}
