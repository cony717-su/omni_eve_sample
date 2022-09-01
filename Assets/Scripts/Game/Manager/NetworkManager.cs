using System;
using System.Collections.Generic;
using System.Web;
using UnityEngine;

public class NetworkManager : IManager<NetworkManager>
{
    public const string CRYPT_KEY = "25a03a9143fedb53dfaceae170b460e9";

    private NetworkConfig _config;
    public NetworkConfig Config => _config;
    
    public void Init()
    {
        _config = NetworkConfig.Create<NetworkConfig>();
    }

    public string Request<TRequest, TResponse>(string action, TRequest reqData, ref TResponse resData)
    {
        string jsonReq = JsonUtility.ToJson(reqData);
        string jsonRes = Request(action, jsonReq);
        
        resData = JsonUtility.FromJson<TResponse>(jsonRes);
        return jsonRes;
    }
    
    public string Request(string action, Dictionary<string, object> reqData)
    {
        string jsonReqData = JsonUtility.ToJson(reqData);
        return Request(action, jsonReqData);
    }

    public string Request(string action, string jsonReq)
    {
        NetworkRequest req = new NetworkRequest(action, jsonReq);
        return req.Request();
    }

    public string RequestApiServer(string action, Dictionary<string, object> reqData)
    {
        string reqWebUrl = Config.ApiServer + action;
        return NetworkRequest.Request(reqData, reqWebUrl);
    }
}