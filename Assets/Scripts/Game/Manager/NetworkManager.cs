using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : IManager<NetworkManager>
{
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

    public string Request(string action, string jsonReqData)
    {
        NetworkRequest req = new NetworkRequest(action, jsonReqData);
        return req.Request();
    }
}