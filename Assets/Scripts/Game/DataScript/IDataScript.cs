using System.IO;
using System.Text;

using UnityEngine;

public class IDataScript
{
    public static string FILE_NAME = "none";
    public static string FullName
    {
        get
        {
            string userConfigDir = Application.persistentDataPath + "\\";
            return userConfigDir + IDataScript.FILE_NAME + ".json";    
        }
    }
    
    public static void Save(ref UserConfigData userConfigData)
    {
        string json = JsonUtility.ToJson(userConfigData);
        DebugManager.Log($"{userConfigData.serverName}, {userConfigData.nickName}");
        using (FileStream fileStream = new FileStream(IDataScript.FullName, FileMode.Create))
        {
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();
        }
    }
    public static void Load(ref UserConfigData userConfigData)
    {
        // Load User Config (serverName, nickName)
        FileInfo userConfigFileInfo = new FileInfo(IDataScript.FullName);
        if (userConfigFileInfo.Exists)
        {
            using (FileStream fileStream = new FileStream(IDataScript.FullName, FileMode.Open))
            {
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                fileStream.Close();

                string json = Encoding.UTF8.GetString(buffer);
                userConfigData = JsonUtility.FromJson<UserConfigData>(json);
            }
        }
    }
}
