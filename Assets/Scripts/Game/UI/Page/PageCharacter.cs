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
        
        ContentsObject content = new ContentsObject(ContentsObjectType.Character, 10100004, 1);
        ContentsIcon icon = GetComponent<ContentsIcon>();
        if (icon == null)
        {
            Debug.Log("ContentsIcon is null");
            return;
        }
        icon.SetContentsIcon(content, "icon_slot_01");

        ContentsObject contentItem = new ContentsObject(ContentsObjectType.Item, 1010035, 1);
        icon.SetContentsIcon(contentItem, "icon_slot_02");
    }
    
    public override void OnShow()
    {
        //UIGenerator.Instance.SetText("text_start", "BATTLE_START");
        
    }
    
    public void OnTest()
    {
        Debug.Log("event Test");
    }
}
