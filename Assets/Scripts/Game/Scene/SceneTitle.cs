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
        LocaleManager.LoadLocaleByCSV("locale_character_name", "CHARACTER_NAME_", 1);
        LocaleManager.LoadLocaleByCSV("locale_character_name", "CHARACTER_SKIN_DESCRIPTION_", 2);
        
    }
}
