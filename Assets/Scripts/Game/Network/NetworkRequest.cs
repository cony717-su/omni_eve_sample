using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using Network;

public class NetworkRequest
{
    string act;
    string p;
    string os;
    string reqKey;
    string v;
    string apiVersion;
    string appliedWhiteYn;
    string id;

    public string Id
    {
        set => id = value;
    }
    public string V
    {
        set => v = value;
    }

    public static string GameCode => NetworkManager.Instance.Config.GameCode;
    public static string BinaryVersion => NetworkManager.Instance.Config.BinaryVersion;
    public static string ApiServer => NetworkManager.Instance.Config.ApiServer;
    public static string GameServer => NetworkManager.Instance.Config.GameServer;
    public static string CryptKey => NetworkManager.Instance.Config.CryptKey;
    public static string OS => NetworkManager.Instance.Config.OS;
    public static string NickName => NetworkManager.Instance.Config.NickName;
    public static string NfGuid => NetworkManager.Instance.Config.NfGuid;

    public NetworkRequest(string action, string strReq = "")
    {
        act = action;
        id = Crypt.EncryptWithBase64(NfGuid, NetworkManager.CRYPT_KEY);

        if (!string.IsNullOrEmpty(strReq))
        {
            SetRequestData(strReq);
        }
        
        os = OS;
        reqKey = GetRegKey();
        v = BinaryVersion;
        apiVersion = BinaryVersion;
        appliedWhiteYn = "Y";
    }

    public void SetRequestData(string strReq, string cryptKey = "")
    {
        if (string.IsNullOrEmpty(cryptKey))
            cryptKey = CryptKey;
        
        // default added parameter
        strReq = strReq.Replace("}", $"\"z\":{UnityEngine.Random.Range(0, 999999)}" + "}");
        p = Crypt.EncryptWithBase64(strReq, cryptKey);
    }

    public string Request()
    {
        string strReq = GetRequestStreamData();
        string strRes = Request(strReq, GameServer);
        return Crypt.DecodeBase64WithCrypt(strRes, NetworkManager.CRYPT_KEY);
    }

    public static string Request(Dictionary<string, object> reqData, string reqWebUrl)
    {
        string strReq = GetRequestStreamData(reqData);
        return Request(strReq, reqWebUrl);
    }
    
    static string Request(string strReq, string reqWebUrl)
    {
        DebugManager.Log($"req: {strReq}");
        
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(reqWebUrl);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = strReq.Length;
        request.UserAgent = $"DestinyChildForKakao/";
        
        StreamWriter stream = new StreamWriter(request.GetRequestStream());
        stream.Write(strReq);
        stream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        
        string strRes = reader.ReadToEnd(); 
        DebugManager.Log($"res: {strRes}");
        return strRes;
    }

    static string GetRequestStreamData(Dictionary<string, object> reqData)
    {
        string strReq = "";
        foreach (var item in reqData)
        {
            strReq += $"{item.Key}=" + HttpUtility.UrlEncode(Convert.ToString(item.Value)) + "&";
        }
        return strReq.TrimEnd('&');
    }

    string GetRequestStreamData()
    {
        Dictionary<string, object> reqData = new Dictionary<string, object>();
        foreach (var fi in typeof(NetworkRequest).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            reqData.TryAdd(fi.Name, fi.GetValue(this));
        }
        return GetRequestStreamData(reqData);
    }

    string GetRegKey()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds() + UnityEngine.Random.Range(0, 90000).ToString();
    }
}