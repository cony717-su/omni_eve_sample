using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : IManager<GameManager>
{
    public string serverName;
    public string nickName;
    
    void Start()
    {
        DebugManager.Log();
    }
    void Init()
    {
        DebugManager.Log();
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        GameManager.Instance.Init();
    }
}