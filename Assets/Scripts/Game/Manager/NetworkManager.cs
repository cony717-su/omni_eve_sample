using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Network;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using Random = UnityEngine.Random;

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
        ServiceInfo reqData = new ServiceInfo(Config.GameCode);

        
        string jsonReq = JsonUtility.ToJson(reqData.ReqData);
        string patchURL = Request(reqData.Action, jsonReq);

        return patchURL;
    }
    
    public string Request<TRequest, TResponse>(string action, TRequest reqData, ref TResponse resData)
    {
        string jsonReq = JsonUtility.ToJson(reqData);
        string jsonRes = Request(action, jsonReq);
        
        resData = JsonUtility.FromJson<TResponse>(jsonRes);
        return jsonRes;
    }

    public string GetAction(string action)
    {
        return "http://kr.dc.dev.shiftup.co.kr/dev_lem/" + action;
    }

    public string GetRegKey()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds() + Random.Range(0, 90000).ToString();
    }

    public string Request(string action, string jsonReqData)
    {
        Request req = new Request();
        req.z = Random.Range(0, 999999);
        req.act = action; 
        req.p = Crypt.EncryptWithBase64(jsonReqData, Config.CryptKey);
        
        req.os = Config.OS;
        req.reqKey = GetRegKey();
        req.v = Config.BinaryVersion;
        req.apiVersion = Config.BinaryVersion;
        req.appliedWhiteYn = "Y";
        
        const string CRYPT_KEY = "25a03a9143fedb53dfaceae170b460e9";
        req.id = Crypt.EncryptWithBase64(Config.NfGuid, CRYPT_KEY);

        string strReq = "";
        foreach (var fi in typeof(Request).GetFields())
        {
            strReq += $"{fi.Name}=" + HttpUtility.UrlEncode(Convert.ToString(fi.GetValue(req))) + "&";
        }
        strReq = strReq.TrimEnd('&');

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://kr.dc.dev.shiftup.co.kr/dev_lem/");
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = strReq.Length;
        request.UserAgent = "DestinyChildForKakao/";

        DebugManager.Log($"{action}: {strReq}");
        StreamWriter stream = new StreamWriter(request.GetRequestStream());
        stream.Write(strReq);
        stream.Close();

        
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());

        return reader.ReadToEnd();
    }

}

public class Request
{
    public int z;
    public string act;
    public string p;
    public string os;
    public string reqKey;
    public string v;
    public string apiVersion;
    public string appliedWhiteYn;
    public string id;
}

public class RequestBase
{
    public static string GameCode => NetworkManager.Instance.Config.GameCode;
    public static string BinaryVersion => NetworkManager.Instance.Config.BinaryVersion;
    public static string CryptKey => NetworkManager.Instance.Config.CryptKey;
    public static string ApiServer => NetworkManager.Instance.Config.ApiServer;
    public static string OS => NetworkManager.Instance.Config.OS;
    public static string NickName => NetworkManager.Instance.Config.NickName;
    public static string NfGuid => NetworkManager.Instance.Config.NfGuid;

    public string _action;
    
    public string Action => ApiServer + _action;
}

public static class NetworkManagerExtension 
{
    public static string DoLogin(this NetworkManager inst)
    {
        ReqLogin login = new ReqLogin();
        login.l = "ko";                     
        login.v = RequestBase.BinaryVersion;
        login.rf = null; // GetReferrer()
        login.sk = "APPLE_APP_STORE"; // GetStoreKind()
        login.os = "iOS"; // GetOSName()
        login.osv= "UNKNOWN__9200"; // GetOSVersion()
        login.dm = "PC"; // GetDeviceModel()
        login.package_name = "iOS"; // ApiGetPackageName
        login.emulator = false; // API_IsEmulator()
        login.os_modulation = false; // API_IsRootedOS()

        login.pf = 88;
        login.token = "PCTOKEN";
        
        ResLogin response = null;
        return inst.Request("do_login", login, ref response);
    }
}

public class ReqLogin
{
    public string dm;           // GetDeviceModel()
    public bool os_modulation;  // API_IsRootedOS()
    public string sk;           // GetStoreKind()
    public string v;            // BINARY_VERSION
    public string token;
    public int pf;              // PlatformHelper:GetMainLoginPlatform()
    public string os;           // GetOSName()
    public string osv;          // GetOSVersion()
    public string package_name; // ApiGetPackageName()
    public string l;            // GetLocale()
    public bool emulator;       // API_IsEmulator()
    public string rf;           // GetReferrer()
}
public class ResLogin
{
    
}

public class LoginInfo : RequestBase
{
    protected string _action = "do_login";

    private RequestData _reqData;
    public RequestData ReqData => _reqData;

    private ResponseData _resData;
    public ResponseData ResData => _resData;

    public LoginInfo(string gameCode)
    {
        _reqData = new RequestData(gameCode);
    }

    public struct RequestData
    {
        public string gamdCd;
        public string os;
        public string clientVersion;
        public string binaryVersion;
        public string clientNid;

        public RequestData(string gameCode)
        {
            gamdCd = gameCode;
            os = OS;
            clientVersion = BinaryVersion;
            binaryVersion = BinaryVersion;
            clientNid = NfGuid;
        }
    }

    public struct ResponseData
    {
        public string _gameCode;
        public string _os;
        public string _clientVersion;
        public string _binaryVersion;
        public string _clientNid;
    }

}









public class ServiceInfo : RequestBase
{
    protected string _action = "/api_version/getClientVersionInfo";

    private RequestData _reqData;
    public RequestData ReqData => _reqData;
    
    private ResponseData _resData;
    public ResponseData ResData => _resData;
    
    public ServiceInfo(string gameCode)
    {
        _reqData = new RequestData(gameCode);
    }
    
    public struct RequestData
    {
        public string gamdCd;
        public string os;
        public string clientVersion;
        public string binaryVersion;
        public string clientNid;
        public RequestData(string gameCode)
        {
            gamdCd = gameCode;
            os = OS;
            clientVersion = BinaryVersion;
            binaryVersion = BinaryVersion;
            clientNid = NfGuid;
        }
    }
    
    public struct ResponseData
    {
        public string _gameCode;
        public string _os;
        public string _clientVersion;
        public string _binaryVersion;
        public string _clientNid;
    }
}