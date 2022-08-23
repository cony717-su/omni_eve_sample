using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SceneTitle : IScene
{
    protected override void OnInit()
    {
        StaticManager.Instance.Init();
    }

    private void Start()
    {
        StaticManager.Instance.Load();
        LocaleManager.LoadLocale("locale");
    }
}
