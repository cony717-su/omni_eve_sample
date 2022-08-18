using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register
{
    public List<Event> registerEventList;

    public void RegisterEvent(object caller)
    {
        if (registerEventList == null)
        {
            return;
        }
        
        foreach(Event registerEvent in registerEventList)
        {
            EventManager.Instance.RegisterHandler(registerEvent, caller);
        }
    }

    public void DeRegisterEvent(object caller)
    {
        if (registerEventList == null)
        {
            return;
        }
        
        foreach(Event registerEvent in registerEventList)
        {
            EventManager.Instance.DeregisterHandler(registerEvent, caller);
        }
    }
}
