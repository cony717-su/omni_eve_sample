using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTitle : IScene
{
    private void Start()
    {
        StaticManager.Instance.Load();
    }
}
