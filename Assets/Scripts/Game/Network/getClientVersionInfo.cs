using System.Collections.Generic;

public static partial class NetworkManagerExtension
{
    public static string getClientVersionInfo(this NetworkManager inst)
    {
        string strReq = NetworkRequest.GetRequestStreamData(new Dictionary<string, object>
        {
            { "gameCd", inst.Config.GameCode },
            { "os", inst.Config.OS },
            { "clientVersion", inst.Config.BinaryVersion },
            { "binaryVersion", inst.Config.ClientVersion },
            { "clientNid", inst.Config.NfGuid },
            { "packageSignature", "" },
        });
        string reqWebUrl = "https://dev-api-integ.line.games/api/api_version/getClientVersionInfo";
        return NetworkRequest.RequestWebUrl(strReq, reqWebUrl);
    }
}