using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStart : UIBase
{
    // Start is called before the first frame update
    void Start()
    {
        SetButton(this.OnButtonTest);
    }

    public void OnButtonTest()
    {
        EventManager.Instance.Send(Event.Test);
        SetText("text_start","BATTLE_START");
        Debug.Log("Clicked btn_start");
    }

    public void OnClickRegister()
    {
        Debug.Log("Registered OnClick()");
        var a = StaticManager.Instance.Get<StaticItem>(1010021);
    }
}
