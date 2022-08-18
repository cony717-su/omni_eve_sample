using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualBasic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : IManager<EventManager> 
{
    private Dictionary<Event, List<object>> _callbackHandler;

    public void Init()
    {
        _callbackHandler = new Dictionary<Event, List<object>>();
    }
    
    public void RegisterHandler(Event eventName, object caller)
    {
        if (!_callbackHandler.ContainsKey(eventName))
        {
            List<object> objectList = new List<object>();
            objectList.Add(caller);
            _callbackHandler.Add(eventName, objectList);
        }
        else
        {
            List<object> objectList = _callbackHandler[eventName];
            if (objectList.Contains(caller))
            {
                return;
            }
            
            objectList.Add(caller);
            _callbackHandler[eventName] = objectList;
        }
    }

    public void DeregisterHandler(Event eventName, object caller)
    {
        if (!_callbackHandler.ContainsKey(eventName))
        {
            return;
        }
        
        List<object> objectList = _callbackHandler[eventName];
        objectList.Remove(caller);
        _callbackHandler[eventName] = objectList;
    }

    public void Send(Event eventName, params object[] parameters)
    {
        if (!_callbackHandler.ContainsKey(eventName))
        {
            return;
        }
        
        List<object> objectList = _callbackHandler[eventName];

        foreach (object eventObject in objectList)
        {
            Type t = eventObject.GetType();
            string eventFunc = "On" + eventName.ToString();
            MethodInfo mi = t.GetMethod(eventFunc);

            if (null == mi)
            {
                continue;
            }
            mi.Invoke(eventObject, parameters);
        }
    }
}
