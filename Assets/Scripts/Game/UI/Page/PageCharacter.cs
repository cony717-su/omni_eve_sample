using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class PageCharacter : Page
{
    public override void OnAwake()
    {
        register.registerEventList = new List<Event>(new Event[]
        {
            Event.Test
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        UIGenerator.Instance.SetButton("btn_start", () =>
        {
            EventManager.Instance.Send(Event.Test);
            UIGenerator.Instance.SetText("text_start", "BATTLE_START");
            Debug.Log("Clicked btn_start");
        });
    }
    
    public override void OnShow()
    {
    }
    
    public void OnTest()
    {
        Debug.Log("event Test");
    }
}
