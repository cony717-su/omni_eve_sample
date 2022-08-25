using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : IManager<GameManager>
{
    public string serverName;
    public string nickName;

    
    void Start()
    {
        InitScene();
        DebugManager.Log();
    }
    void Init()
    {
        StaticManager.Instance.Init();
        DebugManager.Log();
    }
    
    void InitScene()
    {
        string sceneName = GameSceneManager.Instance.ActiveSceneName;
        if (GameObject.Find(sceneName))
            return;

        Type sceneType = Type.GetType(sceneName);
        GameObject gameObject = new GameObject(sceneName);
        gameObject.AddComponent(sceneType);
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        GameManager.Instance.Init();
    }
}