using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Util;

public class UIGenerator : IManager<UIGenerator>
{
    public void SetButton(string btnObjName, UnityAction callback)
    {
        GameObject btnObj = GameObject.Find(btnObjName);

        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(callback);
    }

    public void SetText(string textObjName, string key = "")
    {
        GameObject textObj = GameObject.Find(textObjName);

        Text txt = textObj.GetComponent<Text>();
        string locale = txt.text;
        txt.text = Util.Util.GetLocaleText(key);
    }
}
