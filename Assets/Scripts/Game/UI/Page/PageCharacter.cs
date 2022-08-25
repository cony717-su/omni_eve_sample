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
    
    public void OnTest()
    {
        Debug.Log("eventHandler Test");
    }
}
