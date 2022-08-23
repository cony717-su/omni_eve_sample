using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInit : IScene
{
    public string TargetSceneName = "SceneTitle";
    
    void Start()
    {
        MoveToScene(TargetSceneName);
    }
    protected override void OnInit()
    {

    }
}
