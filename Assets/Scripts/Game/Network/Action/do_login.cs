public static partial class NetworkManagerExtension 
{
    public static string DoLogin(this NetworkManager inst)
    {
        ReqLogin login = new ReqLogin();
        login.l = "ko";                     
        login.v = inst.Config.BinaryVersion;
        login.rf = null;                        // GetReferrer()
        login.sk = "APPLE_APP_STORE";           // GetStoreKind()
        login.os = "iOS";                       // GetOSName()
        login.osv= "UNKNOWN__9200";             // GetOSVersion()
        login.dm = "PC";                        // GetDeviceModel()
        login.package_name = "iOS";             // ApiGetPackageName
        login.emulator = false;                 // API_IsEmulator()
        login.os_modulation = false;            // API_IsRootedOS()

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