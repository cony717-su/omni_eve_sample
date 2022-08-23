using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Util
{
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second) 
        {
            this.First = first;
            this.Second = second;
        }
        public T First { get; set; }
        public U Second { get; set; }
    };
    
    public class Util
    {
        public static string GetLocaleText(string key)
        {
            return LocaleManager.GetLocale(key);
        }

        enum TextLoadingState
        {
            LoadingFront,
            LoadingKey,
            LoadingCenter,
            LoadingValue,
        };
        
        public static Pair<string, string> SplitTextLineByKeyAndValue(string textLine)
        {
            Pair<string, string> textPair = new Pair<string, string>();
            TextLoadingState loadingState = TextLoadingState.LoadingFront;

            string strKey = "";
            string strValue = "";

            bool isSkippingSpace = false;
            for (int i = 0; i < textLine.Length; ++i)
            {
                char c = textLine[i];

                if ('\n' == c || 13 == c)
                    break;

                if (TextLoadingState.LoadingFront == loadingState)
                {
                    if (' ' != c && '\t' != c)
                    {
                        loadingState = TextLoadingState.LoadingKey;
                    }
                }
                if (TextLoadingState.LoadingKey == loadingState)
                {
                    if ('"' == c)
                    {
                        isSkippingSpace = !isSkippingSpace;
                    }
                    else if ((!isSkippingSpace && ' ' == c) || '\t' == c || '=' == c)
                    {
                        loadingState = TextLoadingState.LoadingCenter;
                    }
                    else
                    {
                        strKey += c;
                    }
                }
                if (TextLoadingState.LoadingCenter == loadingState)
                {
                    if (' ' != c && '\t' != c && '=' != c)
                    {
                        loadingState = TextLoadingState.LoadingValue;
                    }
                }
                if (TextLoadingState.LoadingValue == loadingState)
                {
                    if ('"' == c)
                    {
                        continue;
                    }

                    strValue += c;
                }
            }

            textPair.First = strKey;
            textPair.Second = strValue;

            return textPair;
        }

        public static List<string> SplitText(string textLine, string splitUnit)
        {
            List<string> textList = new List<string>();

            int repeatSize = splitUnit.Length;
            int startPos = 0;
            
            while (true)
            {
                if (textLine.Length <= startPos)
                {
                    break;
                }

                int findPos = textLine.IndexOf(splitUnit, startPos);
                if (findPos == -1)
                {
                    findPos = textLine.Length;
                }

                string splitedString = textLine.Substring(startPos, findPos - startPos);
                textList.Add(splitedString);
                startPos = findPos + repeatSize;
            }

            return textList;
        }
    }
}
