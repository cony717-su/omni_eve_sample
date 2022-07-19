using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StaticManager : IManager<StaticManager>
{
    private StaticLoader _StaticLoader;
    private void Start()
    {
        _StaticLoader = gameObject.AddComponent<StaticLoader>();
    }

    public void Load()
    {
        DebugManager.Log();
    }
}
