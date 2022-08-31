using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using Network;
using Newtonsoft.Json;

public class NetworkRequest
{
    int z;
    string act;
    string p;
    string os;
    string reqKey;
    string v;
    string apiVersion;
    string appliedWhiteYn;
    string id;
    
    public static string GameCode => NetworkManager.Instance.Config.GameCode;
    public static string BinaryVersion => NetworkManager.Instance.Config.BinaryVersion;
    public static string CryptKey => NetworkManager.Instance.Config.CryptKey;
    public static string ApiServer => NetworkManager.Instance.Config.ApiServer;
    public static string OS => NetworkManager.Instance.Config.OS;
    public static string NickName => NetworkManager.Instance.Config.NickName;
    public static string NfGuid => NetworkManager.Instance.Config.NfGuid;

    public NetworkRequest(string action, string reqData)
    {
        z = UnityEngine.Random.Range(0, 999999);
        act = action; 
        p = Crypt.EncryptWithBase64(reqData, CryptKey);
        
        os = OS;
        reqKey = GetRegKey();
        v = BinaryVersion;
        apiVersion = BinaryVersion;
        appliedWhiteYn = "Y";
    }
    
    public string Request()
    {
        const string CRYPT_KEY = "25a03a9143fedb53dfaceae170b460e9";
        id = Crypt.EncryptWithBase64(NfGuid, CRYPT_KEY);
        
        string strRes = RequestWebUrl(GetRequestStreamData(), "http://kr.dc.dev.shiftup.co.kr/dev_lem/");
        return Crypt.DecodeBase64WithCrypt(strRes, CRYPT_KEY);
    }

    public static string RequestWebUrl(string strReq, string reqWebUrl)
    {
        DebugManager.Log($"{strReq}");
        
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
        return reader.ReadToEnd();
    }

    public static string GetRequestStreamData(Dictionary<string, object> reqData)
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
        string strReq = "";
        foreach (var fi in typeof(NetworkRequest).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
        {
            strReq += $"{fi.Name}=" + HttpUtility.UrlEncode(Convert.ToString(fi.GetValue(this))) + "&";
        }
        return strReq.TrimEnd('&');
    }

    string GetRegKey()
    {
        return DateTimeOffset.Now.ToUnixTimeSeconds() + UnityEngine.Random.Range(0, 90000).ToString();
    }
}