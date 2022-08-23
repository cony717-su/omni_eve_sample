using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public abstract class IScene : MonoBehaviour
{
    protected void MoveToScene(string targetSceneName)
    {
        GameSceneManager.Instance.MoveToScene(targetSceneName);
    }

    public void Init()
    {
        OnInit();
    }
    protected abstract void OnInit();
}