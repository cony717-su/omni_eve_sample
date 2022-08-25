using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Util;

public class UIBase : MonoBehaviour
{
    public void SetButton(UnityAction callback)
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(callback);
    }

    public void SetText(string textObjName, string key = "")
    {
        GameObject textObj = GameObject.Find(textObjName);

        Text txt = textObj.GetComponent<Text>();
        txt.text = Util.Util.GetLocaleText(key);
    }
}
