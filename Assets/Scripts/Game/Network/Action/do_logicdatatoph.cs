public static partial class NetworkManagerExtension
{
    public static string DoLogicDataOph(this NetworkManager inst)
    {
        NetworkRequest req = new NetworkRequest("do_logicdatatoph");
        req.V = "0";    // GetNumericVersion()
        req.SetRequestData("{}", NetworkManager.CRYPT_KEY);
        return req.Request();
    }
}