using UnityEditor.PackageManager.Requests;

public class NetworkManager : IManager<NetworkManager>
{
    private NetworkConfig _config;
    public NetworkConfig Config => _config;
    public void Init()
    {
        _config = NetworkConfig.Create<NetworkConfig>();
    }

    public string GetServiceInfo()
    {
        RequestServiceInfo req = new RequestServiceInfo(Config.GameCode);

        string patchURL = "";

        return patchURL;
    }
}

public class RequestBase
{
    protected const string action = "";
    
    protected static string GameCode => NetworkManager.Instance.Config.GameCode;
    protected static string BinaryVersion => NetworkManager.Instance.Config.BinaryVersion;
    protected static string CryptKey => NetworkManager.Instance.Config.CryptKey;
    protected static string ApiServer => NetworkManager.Instance.Config.ApiServer;
    protected static string OS => NetworkManager.Instance.Config.OS;
    protected static string NickName => NetworkManager.Instance.Config.NickName;
    protected static string NfGuid => NetworkManager.Instance.Config.NfGuid;

    public string Action => ApiServer + action;
}

public class RequestServiceInfo : RequestBase
{
    protected const string action = "/api_version/getClientVersionInfo";
    public string _gameCode;
    public string _os;
    public string _clientVersion;
    public string _binaryVersion;
    public string _clientNid;

    public RequestServiceInfo(string gameCode)
    {
        _gameCode = gameCode;
        _os = OS;
        _clientVersion = BinaryVersion;
        _binaryVersion = BinaryVersion;
        _clientNid = NfGuid;
    }
}

public class ResponseBase
{
    
}
public class ResponseServiceInfo : ResponseBase
{
    public string _gameCode;
    public string _os;
    public string _clientVersion;
    public string _binaryVersion;
    public string _clientNid;
    public string _packageSignature;
}
