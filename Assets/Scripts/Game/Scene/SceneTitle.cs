using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
// Reference the Unity Analytics namespace
using UnityEngine.Analytics;

public class SceneTitle : IScene
{
    protected override void OnInit()
    {
        StaticManager.Instance.Init();
    }

    private void Start()
    {
        Init();
    }
}