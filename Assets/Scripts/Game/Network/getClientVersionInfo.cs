using System.Collections.Generic;

public static partial class NetworkManagerExtension
{
    public static string getClientVersionInfo(this NetworkManager inst)
    {
        return inst.RequestApiServer("api_version/getClientVersionInfo",new Dictionary<string, object>
        {
            { "gameCd", inst.Config.GameCode },
            { "os", inst.Config.OS },
            { "clientVersion", inst.Config.BinaryVersion },
            { "binaryVersion", inst.Config.ClientVersion },
            { "clientNid", inst.Config.NfGuid },
            { "packageSignature", "" },
        });
    }
}