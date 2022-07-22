using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Util;

public class LocaleManager: IManager<GameManager>
{
    private static Dictionary<string, string> LocaleDictionary { set; get; }
    
    public static void LoadLocale(string fileName)
    {
        LocaleDictionary = new Dictionary<string, string>();
        
        ResourcesManager.LoadAddressableAsset<TextAsset>(fileName, (result) =>
        {
            var locales = result.text;

            string strLine = "";
            for (int i = 0; i < locales.Length; ++i)
            {
                strLine = strLine + locales[i];
            
                if (locales[i] != '\n')
                {
                    continue;
                }

                if (IsInvalidLine(strLine))
                {
                    strLine = "";
                    continue;
                }

                Pair<string, string> textPair = Util.Util.SplitTextLineByKeyAndValue(strLine);

                if (LocaleDictionary.ContainsKey(textPair.First))
                {
                    DebugManager.Log($"Already exist in LocaleDictionary: {textPair.First}");
                    strLine = "";
                    continue;
                }
                
                LocaleDictionary.Add(textPair.First, textPair.Second);
                strLine = "";
            }
        });
    }


    public static void LoadLocaleByCSV(string fileName, string prefix, int index)
    {
        ResourcesManager.LoadAddressableAsset<TextAsset>(fileName, (result) =>
        {
            var locales = result.text;

            string strLine = "";
            for (int i = 0; i < locales.Length; ++i)
            {
                strLine = strLine + locales[i];
            
                if (locales[i] != '\n')
                {
                    continue;
                }

                if (IsInvalidLine(strLine))
                {
                    strLine = "";
                    continue;
                }

                List<string> textList = Util.Util.SplitText(strLine, "\t");

                if (textList.Count < 2)
                {
                    strLine = "";
                    continue;
                }

                if (textList.Count <= index)
                {
                    strLine = "";
                    continue;
                }

                string strKey = prefix + textList[0];
                if (LocaleDictionary.ContainsKey(strKey))
                {
                    DebugManager.Log($"Already exist in LocaleDictionary: {strKey}");
                    strLine = "";
                    continue;
                }

                string strValue = "";
                string textListElement = textList[index];
                if (textListElement.Length > 2 && textListElement[0] == '"')
                {
                    strValue = textListElement.Substring(1, textListElement.Length - 2);
                }
                else
                {
                    strValue = textListElement;
                }
                
                LocaleDictionary.Add(strKey, strValue);
                strLine = "";
            }
        });
    }
    
    private static bool IsInvalidLine(string strLine)
    {
        if (strLine.Length < 2)
        {
            return true;
        }
        else if ('/' == strLine[0] && '/' == strLine[1])
        {
            return true;
        }

        return false;
    }
}
